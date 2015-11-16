#include "StdAfx.h"
#include "TAPIServerConnection.h"
#include "resource.h"
#include "BBjTAPIClientDlg.h"

CTAPIServerConnection::CTAPIServerConnection(void)
{
		m_fTryingConnect=FALSE;
		m_fConnected=FALSE;
		
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
		AfxMessageBox(msg.Right(msg.GetLength()-8));
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
