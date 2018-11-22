#include "pch.h"
#include <thread>

constexpr int PORT = 27893;

#pragma comment (lib, "WS2_32.lib")

namespace NetIO
{
	NetIO::NetIO() :
		m_connectSocket(NULL), m_serverAddr(),
		m_recvAvailable(nullptr), m_recvLocker(), m_recvPackets(),
		m_sendAvailable(nullptr), m_sendLocker(), m_sendPackets(),
		m_isConnected(false),
		m_recvDriver(), m_sendDriver()
	{
		// 创建控制IO缓冲队列的信号量
		m_sendAvailable = CreateEvent(NULL, FALSE, FALSE, NULL);
		ResetEvent(m_sendAvailable);
		m_recvAvailable = CreateEvent(NULL, FALSE, FALSE, NULL);
		ResetEvent(m_recvAvailable);
	}

	NetIO::~NetIO()
	{
		Disconnect();

		CloseHandle(m_sendAvailable);
		CloseHandle(m_recvAvailable);
	}

	bool NetIO::IsConnected() const
	{
		return m_isConnected;
	}

	bool NetIO::Connect()
	{
		try
		{
			if (!_InitNetwork_())
				throw std::exception{ "Failed to set the network environment." };

			m_connectSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
			if (m_connectSocket == INVALID_SOCKET)
			{
				WSACleanup();
				return false;
			}

			OutputDebugStringA("初始化网络环境成功。\n");

			m_serverAddr.sin_family = AF_INET;
			m_serverAddr.sin_addr.S_un.S_addr = inet_addr("10.201.6.248");
			m_serverAddr.sin_port = htons(PORT);
			if (connect(m_connectSocket, (LPSOCKADDR)&m_serverAddr, sizeof(SOCKADDR))
				== SOCKET_ERROR)
			{
				WSACleanup();
				closesocket(m_connectSocket);
				return false;
			}
			m_isConnected = true;

			m_recvDriver 
				= std::move(Thread{ std::bind(&NetIO::_RecvThread_, this) });
			m_sendDriver 
				= std::move(Thread{ std::bind(&NetIO::_SendThread_, this) });
		}
		catch (const std::exception& e)
		{
			WSACleanup();
			OutputDebugStringA(e.what());
			return false;
		}
		return true;
	}

	bool NetIO::Disconnect()
	{
		// 销毁IO通信线程
		if (m_recvDriver.joinable())
			m_recvDriver.join();

		if (m_sendDriver.joinable())
			m_sendDriver.join();

		// 清空IO缓冲队列
		m_recvLocker.lock();
		while (!m_recvPackets.empty())
			m_recvPackets.pop();
		m_recvLocker.unlock();

		m_sendLocker.lock();
		while (!m_sendPackets.empty())
			m_sendPackets.pop();
		m_sendLocker.unlock();

		// 关闭通信
		closesocket(m_connectSocket);
		WSACleanup();

		return true;
	}

	bool NetIO::SendPacket(const Packet& packet)
	{
		m_sendLocker.lock();

		m_sendPackets.push(packet);
		SetEvent(m_sendAvailable);

		m_sendLocker.unlock();
		return false;
	}

	Packet NetIO::ReadPacket()
	{
		Packet recvPacket;
		WaitForSingleObject(m_recvAvailable, 1000);

		m_recvLocker.lock();
		ResetEvent(m_recvAvailable);

		if (!m_recvPackets.empty())
		{
			recvPacket = m_recvPackets.front();
			m_recvPackets.pop();
		}

		m_recvLocker.unlock();
		return recvPacket;
	}

	bool NetIO::_InitNetwork_()
	{
		WSADATA wsaData;
		if (WSAStartup(MAKEWORD(2, 2), &wsaData) != NULL)
			return false;
		return true;
	}

	void NetIO::_RecvThread_()
	{
		int iRecvBytes = 0;
		Packet recvPacket;
		while (m_isConnected)
		{
			std::memset((LPCH)(&recvPacket), 0x0, sizeof(recvPacket));
			iRecvBytes =
				recv(m_connectSocket, (LPCH)(&recvPacket), sizeof(recvPacket), 0);
			if (iRecvBytes > 0)
			{
				m_recvLocker.lock();
				m_recvPackets.push(recvPacket);
				SetEvent(m_recvAvailable);
				m_recvLocker.unlock();
			}
			else
			{
				m_isConnected = false;
				return;
			}
		}
	}

	void NetIO::_SendThread_()
	{
		Packet sendPacket;
		int iSendBytes = 0;
		while (m_isConnected)
		{
			WaitForSingleObject(m_sendAvailable, 1000);

			bool sendValid = false;
			m_sendLocker.lock();
			ResetEvent(m_sendAvailable);

			if (!m_sendPackets.empty())
			{
				sendValid = true;
				sendPacket = m_sendPackets.front();
				m_sendPackets.pop();
			}

			if (sendPacket.type == Packet::Type::LOGOUT)
				m_isConnected = false;

			m_sendLocker.unlock();
			if (sendValid && sendPacket.type != Packet::Type::INVALID)
			{
				iSendBytes = send(m_connectSocket,
					(LPCH)(&sendPacket),
					sizeof(sendPacket),
					0);

				if (iSendBytes == SOCKET_ERROR)
				{
					m_isConnected = false;
					return;
				}
			}
		}
	}
}

