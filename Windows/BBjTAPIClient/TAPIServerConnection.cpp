#include "StdAfx.h"
#include "TAPIServerConnection.h"
#include "resource.h"
#include "BBjTAPIClientDlg.h"
#include <afxmt.h>
#include <tapi.h>
#include "atapi.h"
#include "Phone.h"

CTAPIServerConnection::CTAPIServerConnection(void)
{
		m_fTryingConnect=FALSE;
		m_fConnected=FALSE;
		OpenLine=NULL;
		BuildTAPIData();
		
		
}

CTAPIServerConnection::~CTAPIServerConnection(void)
{
}



void CTAPIServerConnection::SocketErrorMessage()
{
int i=GetLastError();
switch (i)
	{
	case '0':
		return;
	case WSANOTINITIALISED:
		AfxMessageBox(IDS_WSANOTINITIALISED);
		break;
	case WSAENETDOWN:
		AfxMessageBox(IDS_WSAENETDOWN);
		break;
	case WSAEACCES:
		AfxMessageBox(IDS_WSAEACCES);
		break;
	case WSAEINTR:
		AfxMessageBox(IDS_WSAEINTR);
		break;
	case WSAEINPROGRESS:
		AfxMessageBox(IDS_WSAEINPROGRESS);
		break;
	case WSAEFAULT:
		AfxMessageBox(IDS_WSAEFAULT);
		break;
	case WSAENETRESET:
		AfxMessageBox(IDS_WSAENETRESET);
		break;
	case WSAENOBUFS:
		AfxMessageBox(IDS_WSAENOBUFS);
		break;
	case WSAENOTCONN:
		AfxMessageBox(IDS_WSAENOTCONN);
		break;
	case WSAENOTSOCK:
		AfxMessageBox(IDS_WSAENOTSOCK);
		break;
	case WSAEOPNOTSUPP:
		AfxMessageBox(IDS_WSAEOPNOTSUPP);
		break;
	case WSAESHUTDOWN:
		AfxMessageBox(IDS_WSAESHUTDOWN);
		break;
	case WSAEWOULDBLOCK:
		return;
		AfxMessageBox(IDS_WSAEWOULDBLOCK);
		break;
	case WSAEMSGSIZE:
		AfxMessageBox(IDS_WSAEMSGSIZE);
		break;
	case WSAEINVAL:
		AfxMessageBox(IDS_WSAEINVAL);
		break;
	case WSAECONNABORTED:
		AfxMessageBox(IDS_WSAECONNABORTED);
		break;
	case WSAECONNRESET:
		AfxMessageBox(IDS_WSAECONNRESET);
		break;
	case WSAETIMEDOUT:
		AfxMessageBox(IDS_WSAETIMEDOUT);
		break;
	case WSAENETUNREACH:
		AfxMessageBox(IDS_WSAENETUNREACH);
		break;
	case WSAEMFILE:
		AfxMessageBox(IDS_WSAEMFILE);
		break;
	case WSAEISCONN:
		AfxMessageBox(IDS_WSAEISCONN);
		break;
/*	case WSAEDESTADDREQ:
		AfxMessageBox(IDS_WSAEDESTADDREQ);
		break;*/
	case WSAECONNREFUSED:
		AfxMessageBox(IDS_WSAECONNREFUSED);
		break;
	case WSAEAFNOSUPPORT:
		AfxMessageBox(IDS_WSAEAFNOSUPPORT);
		break;
	case WSAEADDRNOTAVAIL:
		AfxMessageBox(IDS_WSAEADDRNOTAVAIL);
		break;
	case WSAEADDRINUSE:
		AfxMessageBox(IDS_WSAEADDRINUSE);
		break;
	default:
		AfxMessageBox(IDS_WSADEFAULT);
		break;
	}



}



void CTAPIServerConnection::Reconnect(CString host,CString port, CString extension)
{
	LastMessage="Trying to connect to "+host;
	dlg->UpdateData();
	this->host = host;
	this->port = atoi(port);
	this->extension = extension;
	Close();
	Create();
	Connect(host,this->port);
}



void CTAPIServerConnection::OnSend(int nErrorCode)
{
	int i;
	i=nErrorCode;
}
void CTAPIServerConnection::OnReceive(int nErrorCode)
{
	char buf[10000];
	int i=Receive(&buf,10000);
	buf[i]=0;
	CString msg = buf;
	if (msg.Left(8)=="OUTCALL:") {
		CString Number = msg.Right(msg.GetLength()-8);
		MakeCall(Number);
	}
	//m_pData->OnReceive(buf);
	
}

void CTAPIServerConnection::OnConnect(int nErrorCode)
{
	if (nErrorCode == 0){
		m_fConnected=TRUE;
		LastMessage="";
		CString cmd = "REG:"+this->extension+"\n";
		Send(cmd.GetBuffer(),cmd.GetLength());
	}
	else
	{
		LastMessage="Can't connect to Server";
	}

	if (dlg != NULL){
		dlg->UpdateDisplay();
	}

}

void CTAPIServerConnection::OnClose(int nErrorCode)
{
	m_fConnected=FALSE;
	if (dlg != NULL){
		dlg->UpdateDisplay();
	}
}

void CTAPIServerConnection::IncomingCall(CString number)
{
	if (m_fConnected){
		CString m = "CALL:"+number+"\n";
		Send(m.GetBuffer(),m.GetLength());
	};


}

void CTAPIServerConnection::BuildTAPIData()
{

    // Initialize a connection with TAPI and determine if there 
    // are any TAPI complient devices installed.
    if (GetTAPIConnection()->Init("BBjTAPIClient", 
			RUNTIME_CLASS(CMyLine), NULL, RUNTIME_CLASS(CMyCall),
			RUNTIME_CLASS(CMyPhone)) != 0 ||
        GetTAPIConnection()->GetLineDeviceCount() == 0)
    {
		
        AfxMessageBox ("There are no TAPI devices installed!");
		return;
    }

	// Load our line devices with all the detected TAPI lines.
	
	Devices.RemoveAll();
	DeviceObjects.RemoveAll();

	for (DWORD dwLine = 0; dwLine < GetTAPIConnection()->GetLineDeviceCount(); dwLine++)
	{
		CTapiLine* pLine = GetTAPIConnection()->GetLineFromDeviceID(dwLine);
		if (pLine != NULL)
		{
			Devices.Add(pLine->GetLineName());
			DeviceObjects.Add(pLine);
		}
	}		
}

void CTAPIServerConnection::SelectLine(int line)
{
	Addresses.RemoveAll();
	AddressObjects.RemoveAll();

	SelectedLine = line;
	CTapiLine* pLine = (CTapiLine*)(DeviceObjects.GetAt(line));

	for (DWORD dwAddress = 0; dwAddress < pLine->GetAddressCount(); dwAddress++)
		{
			CTapiAddress* pAddr = pLine->GetAddress(dwAddress);
			CString strName = pAddr->GetDialableAddress();
			if (strName.IsEmpty())
				strName.Format("Address %ld", dwAddress);
			Addresses.Add(strName);
			AddressObjects.Add(pAddr);
		}
}

void CTAPIServerConnection::SelectAddress(int address)
{
	SelectedAddress = address;
}


void CTAPIServerConnection::StartTAPISession() 
{
	// Priority of media modes
	static struct
	{
		DWORD dwMediaMode;
		LPCSTR pszName;
	} g_MediaModes[] = {
		{ LINEMEDIAMODE_INTERACTIVEVOICE,	"Voice" },
		{ LINEMEDIAMODE_DATAMODEM,			"DataModem" },
		{ LINEMEDIAMODE_AUTOMATEDVOICE,		"AutomatedVoice" }, 
		{ LINEMEDIAMODE_DIGITALDATA,		"DigitalData" },
		{ LINEMEDIAMODE_G3FAX,				"G3 FAX" },
		{ LINEMEDIAMODE_G4FAX,				"G4 FAX" },
		{ LINEMEDIAMODE_TDD,				"TDD" },
		{ LINEMEDIAMODE_TELETEX,			"TeleTex" },
		{ LINEMEDIAMODE_VIDEOTEX,			"VideoTex" },
		{ LINEMEDIAMODE_TELEX,				"Telex" },
		{ LINEMEDIAMODE_MIXED,				"Mixed" },
		{ LINEMEDIAMODE_ADSI,				"ADSI" },
		{ LINEMEDIAMODE_VOICEVIEW,			"VoiceView" },
		{ 0, NULL }
	};


	if (OpenLine != NULL)
	{
		CTapiLine* pl = (CTapiLine*)OpenLine;
		if (pl->IsOpen())
		{
			pl->Close();
			OpenLine = NULL;
		}
	}

	CTapiLine* pLine =(CTapiLine*)(DeviceObjects.GetAt(SelectedLine));
	if (pLine == NULL)
		return;

	if (pLine->IsOpen())
	{
		pLine->Close();
	}
	else
	{
		// Open for ALL media modes first.
		DWORD dwMediaMode = 0;
		const LPLINEDEVCAPS lpCaps = pLine->GetLineCaps();
		if (lpCaps)
			dwMediaMode = (lpCaps->dwMediaModes & ~LINEMEDIAMODE_UNKNOWN);

		// Open the line
		LONG lResult = pLine->Open (LINECALLPRIVILEGE_OWNER | LINECALLPRIVILEGE_MONITOR, dwMediaMode);

		// UNIMODEM only allows ONE media mode to be chosen.. pick the best one available.
		if (lResult == LINEERR_INVALMEDIAMODE)
		{
			// Pick only ONE media mode
			for (int i = 0; g_MediaModes[i].dwMediaMode != 0; i++)
			{
				if (dwMediaMode & g_MediaModes[i].dwMediaMode)
				{
					lResult = pLine->Open (LINECALLPRIVILEGE_OWNER | LINECALLPRIVILEGE_MONITOR, g_MediaModes[i].dwMediaMode);
					if (lResult == 0)
					{	
						CString e="Forced to open line with media mode";
						e+=g_MediaModes[i].pszName;
						AfxMessageBox(e);
						break;
					}
				}
			}
		}

		// Show an error
		if (lResult != 0)
			AfxMessageBox("Error opening line");
		else
		{
			// Get the states we get notified on
			DWORD dwAddrSt = (LINEADDRESSSTATE_CAPSCHANGE + LINEADDRESSSTATE_CAPSCHANGE-1);
			CTapiAddress* pAddr = (CTapiAddress*)AddressObjects.GetAt(SelectedAddress);
			if (pAddr)
			{
				LPLINEADDRESSCAPS lpACaps = pAddr->GetAddressCaps();
				if (lpACaps)
					dwAddrSt &= lpACaps->dwAddressStates;
			}
			DWORD dwStates = (LINEDEVSTATE_REMOVED + LINEDEVSTATE_REMOVED-1);
			if (lpCaps) 
				dwStates &= lpCaps->dwLineStates;
			lResult = pLine->SetStatusMessages(dwStates, dwAddrSt);
			
			if (lResult != 0)
				AfxMessageBox("Error lineSetStatusMessages");

			OpenLine = pLine;
			
		}
	}
	

}

void CTAPIServerConnection::MakeCall(CString m_strNumber) 
{
	
	CTapiLine* pLine = (CTapiLine*)OpenLine;
	if (pLine == NULL)
	{
		AfxMessageBox("TAPI is offline");
		return;
	}

	// If this is a predictive dialer then prompt the user for
	// predictive dialer information.
	LPLINECALLPARAMS lpCallParams = NULL;
	CTapiAddress* pAddr = pLine->GetAddress((DWORD)0);
	/*
	if (pAddr != NULL)
	{
		LPLINEADDRESSCAPS lpCaps = pAddr->GetAddressCaps();
		if (lpCaps && (lpCaps->dwAddrCapFlags & LINEADDRCAPFLAGS_PREDICTIVEDIALER) != 0)
		{
			CPredDialDlg dlg(this);
			dlg.m_dwCallStates = lpCaps->dwPredictiveAutoTransferStates;
			dlg.m_nTimeout = lpCaps->dwMaxNoAnswerTimeout;
			if (dlg.DoModal() == IDOK)
			{
				lpCallParams = (LPLINECALLPARAMS) new BYTE[sizeof(LINECALLPARAMS) + dlg.m_strTarget.GetLength()+1];
				memset(lpCallParams, 0, sizeof(LINECALLPARAMS));
				lpCallParams->dwTotalSize = sizeof(LINECALLPARAMS) + dlg.m_strTarget.GetLength()+1;
				lpCallParams->dwBearerMode = LINEBEARERMODE_VOICE;
				lpCallParams->dwMediaMode = LINEMEDIAMODE_INTERACTIVEVOICE;
				lpCallParams->dwCallParamFlags = 0;
				lpCallParams->dwAddressID = 0;
				lpCallParams->dwAddressMode = LINEADDRESSMODE_ADDRESSID;
				lpCallParams->dwPredictiveAutoTransferStates = dlg.m_dwCallStates;
				lpCallParams->dwTargetAddressOffset = sizeof(LINECALLPARAMS);
				lpCallParams->dwTargetAddressSize = dlg.m_strTarget.GetLength()+1;
				lstrcpy((LPTSTR)lpCallParams+lpCallParams->dwTargetAddressOffset, dlg.m_strTarget);
				lpCallParams->dwNoAnswerTimeout = dlg.m_nTimeout;
			}
		}
	}
	*/ 
	CTapiCall* pCall = NULL;
	LONG lResult = pLine->MakeCall(&pCall, m_strNumber, 0, lpCallParams);
	if (lResult != 0)
		AfxMessageBox("error making call to "+m_strNumber);

	delete [] lpCallParams;
}