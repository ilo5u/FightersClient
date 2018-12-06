#pragma once

namespace NetIO
{
	typedef SOCKET      Socket;
	typedef SOCKADDR_IN SockaddrIn;
	typedef HANDLE      Handle;
	typedef std::mutex  Mutex;
	typedef std::thread Thread;

	typedef std::queue<Packet> Packets;

	class NetIO
	{
	public:
		NetIO();
		~NetIO();

	public:
		bool IsConnected() const;
		bool Connect();
		bool Disconnect();

		std::string GetIP() const;
		void SetIP(const std::string& newIP);

		bool   SendPacket(const Packet& packet);
		Packet ReadPacket();

	private:
		Socket     m_connectSocket;
		SockaddrIn m_serverAddr;

		Handle  m_recvAvailable;
		Mutex   m_recvLocker;
		Packets m_recvPackets;

		Handle  m_sendAvailable;
		Mutex   m_sendLocker;
		Packets m_sendPackets;

		bool   m_isConnected;

		Thread m_recvDriver;
		Thread m_sendDriver;

	private:
		bool _InitNetwork_();

		void _RecvThread_();
		void _SendThread_();
	};
	typedef NetIO * HNetIO;
}