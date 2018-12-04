#pragma once

namespace Kernel
{
	typedef ::Pokemen::BattleStage Battle;
	typedef ::Pokemen::BattleMessage BattleMessage;
	typedef ::Pokemen::Pokemen * HPokemen;
	typedef std::list<HPokemen> Pokemens;
	typedef Packet::Type       PacketType;

	public enum struct MsgType
	{
		INVALID = 0x0000,

		LOGIN_REQUEST = 0x0001,
		LOGIN_SUCCESS = 0x0002,
		LOGIN_FAILED = 0x0003,
		LOGON_REQUEST = 0x0004,
		LOGON_SUCCESS = 0x0005,
		LOGON_FAILED = 0x0006,
		LOGOUT = 0x0007,

		PVE_RESULT = 0x0010,
		PVP_REQUEST = 0x0020,
		PVP_FAILED = 0x0030,
		PVP_BATTLE = 0x0040,
		PVP_RESULT = 0x0050,
		PVE_MESSAGE = 0x0060,
		PVP_MESSAGE = 0x0070,
		PVP_UPDATE = 0x0080,

		SET_ONLINE_USERS = 0x0100,
		UPDATE_ONLINE_USERS = 0x0200,
		UPDATE_POKEMENS = 0x0300,
		UPDATE_RANKLIST = 0x0400,
		GET_ONLINE_USERS = 0x0500,
		PROMOTE_POKEMEN = 0x0600,
		ADD_POKEMEN = 0x0700,
		SUB_POKEMEN = 0x0800,

		DISCONNECT = 0xFFFF
	};

	public value struct Message
	{
		MsgType type;
		Platform::String^ data;
	};

	public value struct Property sealed
	{
		int id;
		int type;
		Platform::String^ name;

		int hpoints;
		int attack;
		int defense;
		int agility;

		int interval;
		int critical;
		int hitratio;
		int parryratio;

		int career;
		int exp;
		int level;

		int primarySkill;
		int secondSkill;
	};

	public ref class Pokemen sealed
	{
	public:
		Pokemen(int type, int level);
		Pokemen(int level);
		virtual ~Pokemen();

	public:
		void SetAIPlayer();
		Property GetProperty();

	private:
		::Pokemen::Pokemen instance;
	};

	public ref class Core sealed
	{
	public:
		Core();
		virtual ~Core();

	public:
		void SendMessage(Message msg);
		Message ReadOfflineMessage();
		Message ReadOnlineMessage();

	public:
		bool StartConnection();
		bool CloseConnection();
		bool IsConnected();

	public:
		Property GetPropertyAt(int pokemenId);

	public:
		void SetBattlePlayersAndType(int pokemenId, Kernel::Pokemen^ ai, int type);
		void StartBattle();
		void SetBattleOn();
		void SetBattlePasue();

		// 强制关闭比赛
		void ShutdownBattle();

		bool IsBattleRunning();

	private:
		::NetIO::NetIO netDriver;
		Battle stage;
		Pokemens pokemens;
	};
}
