// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "MinHook.h"

void ThreadEntry(HMODULE);

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
		DisableThreadLibraryCalls(hModule);
		CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE) &ThreadEntry, hModule, 0, NULL);
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

void ThreadEntry(HMODULE dll)
{

	HWND hWnd = GetForegroundWindow();
	DWORD foregroundThreadID = GetWindowThreadProcessId(hWnd, 0);
	DWORD currentThreadID = GetCurrentThreadId();

	while (true)
	{
		Sleep(50);
		AttachThreadInput(foregroundThreadID, currentThreadID, TRUE);
		SetCursor(NULL);
		AttachThreadInput(foregroundThreadID, currentThreadID, FALSE);
	}
}
