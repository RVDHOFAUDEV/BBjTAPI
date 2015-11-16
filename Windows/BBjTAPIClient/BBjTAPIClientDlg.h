// BBjTAPIClientDlg.h : header file
//

#pragma once

#include "TrayDialog.h"
#include "afxwin.h"
#include "TAPIServerConnection.h"
#include "resource.h"

// CBBjTAPIClientDlg dialog
class CBBjTAPIClientDlg : public CTrayDialog
{
// Construction
public:
	CBBjTAPIClientDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_BBJTAPICLIENT_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;
	CTAPIServerConnection* con;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();

	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedButton1();
	afx_msg void OnEnChangeSimuDocall();
	CString Port;
	CString Host;
	CString Status;


	afx_msg void OnBnClickedSimuIncomingcall();

	void UpdateDisplay();
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	CString Ext;
};
