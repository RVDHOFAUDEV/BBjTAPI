#pragma once
#include "afxsock.h"

class CBBjTAPIClientDlg;

class CTAPIServerConnection :
	public CAsyncSocket
{
public:
	CTAPIServerConnection(void);
	~CTAPIServerConnection(void);
	void OnSend(int nErrorCode);
	void OnReceive(int nErrorCode);
	void OnConnect(int nErrorCode);
	void OnClose(int nErrorCode);
	void SocketErrorMessage();
	void Reconnect(CString host, CString port, CString extension);

public:
		BOOL m_fTryingConnect;
		BOOL m_fConnected;
		CBBjTAPIClientDlg* dlg;
		CString LastMessage;
		CString host;
		int port;
		CString extension;

		
public:
	void IncomingCall(CString number);
};
