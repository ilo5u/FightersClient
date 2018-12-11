#include "pch.h"

namespace Pokemen
{
	static const Strings namesOfAssassin{ "Kakuna", "Beedrill", "Pidgey", "Pidgeotto" };

	Assassin::Career::Career(Career::Type type) :
		type(type)
	{
	}

	/// <summary>
	/// 
	/// </summary>
	Assassin::Assassin(int level) :
		BasePlayer(
			PokemenType::ASSASSIN, namesOfAssassin[_Random(namesOfAssassin.size())],
			BasicProperties::hitpoints + _Random(CommonBasicValues::propertyOffset),
			BasicProperties::attack + _Random(CommonBasicValues::propertyOffset),
			BasicProperties::defense + _Random(CommonBasicValues::propertyOffset),
			BasicProperties::agility + _Random(CommonBasicValues::propertyOffset),
			0x0, 0x0, 0x0, 0x0
		),
		m_skill(static_cast<Skill::Type>(_Random(2))),
		m_career(Career::Type::Normal)
	{
		this->m_property.m_interval
			= IntervalValueCalculator(
				CommonBasicValues::interval,
				this->m_property.m_agility,
				this->m_property.m_defense
			);
		this->m_property.m_critical
			= CriticalValueCalculator(
				CommonBasicValues::critical,
				this->m_property.m_agility
			);
		this->m_property.m_hitratio
			= HitratioValueCalculator(
				CommonBasicValues::hitratio,
				this->m_property.m_agility,
				this->m_property.m_attack
			);
		this->m_property.m_parryratio
			= ParryratioValueCalculator(
				CommonBasicValues::parryratio,
				this->m_property.m_agility,
				this->m_property.m_defense
			);

		while (this->m_property.m_level < std::min<Value>(level, CommonBasicValues::levelLimitation))
			Upgrade(CommonBasicValues::exp);

		if (level > 8)
			Promote(static_cast<Career::Type>(_Random(3)));
	}

	Assassin::Assassin(const Property& prop, Career::Type career) :
		BasePlayer(prop),
		m_skill(Skill::Type::TEARING),
		m_career(career)
	{
	}

	/// <summary>
	/// 
	/// </summary>
	Assassin::~Assassin()
	{
	}

	Assassin::Skill::Skill(Type primarySkill) :
		primarySkill(primarySkill)
	{
	}

	Assassin::Career::Type Assassin::GetCareer() const
	{
		return this->m_career.type;
	}

	void Assassin::SetCareer(Career::Type career)
	{
		this->m_career.type = career;
	}

	Assassin::Skill::Type Assassin::GetPrimarySkill() const
	{
		return this->m_skill.primarySkill;
	}

	String Assassin::Attack(BasePlayer& opponent)
	{
		this->m_battleMessage[0] = 0x0;

		/* 状态判决 */
		if (this->InState(State::DEAD))
			return { };

		if (this->InState(State::INSPIRED))
		{
			if (this->m_stateRoundsCnt.inspired == 1)
			{
				this->m_property.m_attack -=
					this->m_effects.inspired.attack;
				this->m_property.m_agility -=
					this->m_effects.inspired.agility;
				this->m_property.m_interval -=
					this->m_effects.inspired.interval;
				this->SubState(State::INSPIRED);
			}
			else
			{
				--this->m_stateRoundsCnt.inspired;
			}
		}

		if (this->InState(State::SILENT))
		{
			if (this->m_stateRoundsCnt.silent == 1)
			{
				this->SubState(State::SILENT);
			}
			else
			{
				--this->m_stateRoundsCnt.silent;
			}
		}

		if (this->InState(State::SLOWED))
		{
			if (this->m_stateRoundsCnt.slowed == 1)
			{
				this->m_property.m_interval -= this->m_effects.slowed.interval;
				this->SubState(State::SLOWED);
			}
			else
			{
				--this->m_stateRoundsCnt.slowed;
			}
		}

		if (this->InState(State::SUNDERED))
		{
			if (this->m_stateRoundsCnt.sundered == 1)
			{
				this->m_property.m_attack -= this->m_effects.sundered.attack;
				this->SubState(State::SUNDERED);
			}
			else
			{
				--this->m_stateRoundsCnt.sundered;
			}
		}

		if (this->InState(State::DIZZYING))
		{
			if (this->m_stateRoundsCnt.dizzying == 1)
			{
				this->SubState(State::DIZZYING);
				return { };
			}
			else
			{
				--this->m_stateRoundsCnt.dizzying;
			}
		}

		/* 攻击判决 */
		if (_Hit_Target(this->m_property.m_hitratio, opponent.GetParryratio()))
		{
			Value damage = this->m_property.m_attack;

			if (_Hit_Target((this->m_property.m_critical + this->m_property.m_agility) / 2, opponent.GetCritical()))
			{ // 暴击
				damage = static_cast<Value>((double)damage * 2.0);
			}

			this->_InitSkill_();
			/* 修正技能效果 */
			int tearingChance = this->m_skill.tearingChance;
			int bleedRounds = CommonBasicValues::bleedRounds;
			int slowChance = this->m_skill.slowChance;
			int slowRounds = CommonBasicValues::slowedRounds;
			int stolenIndex = this->m_skill.stolenIndex;
			switch (this->m_career.type)
			{
			case Career::Type::Yodian:
				tearingChance
					+= ConvertValueByPercent(tearingChance, Career::Yodian::tearingChanceIncIndex);
				slowChance
					+= ConvertValueByPercent(slowChance, Career::Yodian::slowChanceIncIndex);
				stolenIndex
					+= ConvertValueByPercent(stolenIndex, Career::Yodian::stolenIncIndex);
				bleedRounds += Career::Yodian::bleedRoundsIncIndex;
				break;

			case Career::Type::Michelle:
				slowChance
					+= ConvertValueByPercent(slowChance, Career::Michelle::slowChanceIncIndex);
				stolenIndex
					+= ConvertValueByPercent(stolenIndex, Career::Michelle::stolenDecIndex);
				slowRounds += Career::Michelle::slowRoundsIncIndex;
				break;

			default:
				break;
			}

			/* 技能判决 */
			if (!this->InState(State::SILENT) && this->InState(State::ANGRIED))
			{
				sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
					"精神鼓舞。");
				this->m_anger = 0;
				this->SubState(State::ANGRIED);
				/* 精神鼓舞 */
				this->m_effects.inspired.agility =
					ConvertValueByPercent(this->m_property.m_agility, this->m_skill.fastenIndex);
				this->m_effects.inspired.interval = -this->m_skill.slowIndex; // -200
				this->m_stateRoundsCnt.inspired = BasicProperties::inspiredRounds;
				switch (this->m_career.type)
				{
				case Career::Type::Yodian:
					this->m_effects.inspired.attack =
						ConvertValueByPercent(this->m_property.m_attack, Career::Yodian::damageDecIndex);
					this->m_effects.inspired.interval +=
						Career::Yodian::intervalDecIndex;
					break;

				case Career::Type::Michelle:
					this->m_effects.inspired.attack =
						ConvertValueByPercent(this->m_property.m_attack, Career::Michelle::damageIncIndex);
					++this->m_stateRoundsCnt.inspired;
					break;

				default:
					break;
				}
				this->m_property.m_attack += this->m_effects.inspired.attack;
				this->m_property.m_agility += this->m_effects.inspired.agility;
				this->m_property.m_interval += this->m_effects.inspired.interval;
				this->AddState(State::INSPIRED);

				damage +=
					ConvertValueByPercent(ConvertValueByPercent(opponent.GetAnger(), stolenIndex), stolenIndex);
			}
			else if (!this->InState(State::SILENT))
			{
				switch (this->m_skill.primarySkill)
				{
				case Skill::Type::TEARING:
					/* 主修撕裂 */
				{
					if (_Hit_Target(tearingChance, 0))
					{
						/* 撕裂 */
						sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
							"撕裂。");
						opponent.SetBleedRounds(bleedRounds);
						opponent.AddState(State::BLEED);
					}
					else if (_Hit_Target(slowChance, 5)
						&& !opponent.InState(State::SLOWED))
					{
						/* 减速 */
						sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
							"减速。");
						opponent.m_effects.slowed.interval = this->m_skill.slowIndex;
						opponent.m_property.m_interval += opponent.m_effects.slowed.interval;
						opponent.SetSlowedRounds(slowRounds);
						opponent.AddState(State::SLOWED);
					}
				}
				break;

				case Skill::Type::SLOW:
				{
					/* 主修减速 */
					if (_Hit_Target(slowChance, 0)
						&& !opponent.InState(State::SLOWED))
					{
						/* 减速 */
						sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
							"减速。");
						opponent.m_effects.slowed.interval = this->m_skill.slowIndex;
						opponent.m_property.m_interval += opponent.m_effects.slowed.interval;
						opponent.SetSlowedRounds(slowRounds);
						opponent.AddState(State::SLOWED);
					}
					else if (_Hit_Target(tearingChance, 5)
						&& !opponent.InState(State::BLEED))
					{
						/* 撕裂 */
						sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
							"撕裂。");
						opponent.SetBleedRounds(bleedRounds);
						opponent.AddState(State::BLEED);
					}
				}
				break;

				default:
					break;
				}
			}
			if (this->m_career.type == Career::Type::Yodian)
			{
				this->m_anger = std::min<Value>(
					CommonBasicValues::angerLimitation,
					this->m_anger + _Random(CommonBasicValues::angerInc)
					);

				if (this->m_anger == CommonBasicValues::angerLimitation)
					this->AddState(State::ANGRIED);
			}

			// 攻击敌方小精灵
			/* 伤害判决 */
			sprintf(m_battleMessage + std::strlen(m_battleMessage),
				"造成%d点伤害。",
				AttackDamageCalculator(damage, opponent.GetDefense()));
			Value rebounce = opponent.IsAttacked(AttackDamageCalculator(damage, opponent.GetDefense()));
			if (rebounce > 0)
			{	// 对方开启反甲
				sprintf(m_battleMessage + std::strlen(m_battleMessage),
					"受到%d点反伤。", rebounce);
				this->m_property.m_hpoints -= rebounce;
			}

			if (this->m_property.m_hpoints <= 0)
			{
				sprintf(m_battleMessage + std::strlen(m_battleMessage),
					"小精灵死亡。");
				this->m_property.m_hpoints = 0;
				this->m_state = State::DEAD;
			}
		}
		else
		{
			sprintf(m_battleMessage + std::strlen(m_battleMessage), "未命中。");
		}

		return m_battleMessage;
	}

	Value Assassin::IsAttacked(Value damage)
	{
		if (damage >= this->m_property.m_hpoints)
		{
			this->m_property.m_hpoints = 0;
			this->m_state = State::DEAD;
		}
		else
		{
			this->m_property.m_hpoints -= damage;
			this->m_anger = std::min<Value>(
				CommonBasicValues::angerLimitation,
				this->m_anger + CommonBasicValues::angerInc + _Random(CommonBasicValues::angerInc)
				);

			if (this->m_anger == CommonBasicValues::angerLimitation)
				this->AddState(State::ANGRIED);

			/* 出血 */
			if (this->InState(State::BLEED))
			{
				this->m_property.m_hpoints -= BloodingDamageCalculator(CommonBasicValues::bleedDamage, this->m_property.m_defense);
				sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
					"出血造成%d点伤害。",
					BloodingDamageCalculator(CommonBasicValues::bleedDamage, this->m_property.m_defense));
				if (this->m_property.m_hpoints <= 0)
				{
					this->m_property.m_hpoints = 0;
					this->m_state = State::DEAD;
				}

				if (this->m_stateRoundsCnt.bleed == 1)
					this->SubState(State::BLEED);
			}
		}

		return 0;
	}

	bool Assassin::SetPrimarySkill(Skill::Type primarySkill)
	{
		this->m_skill.primarySkill = primarySkill;
		return false;
	}

	bool Assassin::Promote(Career::Type career)
	{
		if (this->m_career.type == Career::Type::Normal)
		{
			this->m_career.type = career;
			int bonus = (this->m_property.m_level - 8) * 10 + 100;
			switch (this->m_career.type)
			{
			case Career::Type::Yodian:
			{
				this->m_property.m_attack
					+= ConvertValueByPercent(this->m_property.m_attack, Career::Yodian::damageDecIndex);
				this->m_property.m_interval
					+= Career::Yodian::intervalDecIndex;
				this->m_property.m_hpoints
					+= ConvertValueByPercent(this->m_property.m_hpoints, Career::Yodian::hpointsDecIndex);
				this->m_property.m_agility
					+= ConvertValueByPercent(ConvertValueByPercent(this->m_property.m_agility, Career::Yodian::agilityIncIndex), bonus);
			}
			break;

			case Career::Type::Michelle:
			{
				this->m_property.m_attack
					+= ConvertValueByPercent(ConvertValueByPercent(this->m_property.m_attack, Career::Michelle::damageIncIndex), bonus);
				this->m_property.m_defense
					+= ConvertValueByPercent(ConvertValueByPercent(this->m_property.m_defense, Career::Michelle::defenseIncIndex), bonus);
			}
			break;

			default:
				break;
			}
		}
		return false;
	}

	const static double PI = 3.1415926;
	void Assassin::_LevelUpPropertiesDistributor_()
	{
		Value attackInc = 0;
		Value defenseInc = 0;
		Value agilityInc = 0;

		switch (this->m_career.type)
		{
		case Career::Type::Normal:
		{
			attackInc
				= _Random(CommonBasicValues::levelupPropertiesInc / 4);
			defenseInc 
				= _Random(CommonBasicValues::levelupPropertiesInc / 4);
			agilityInc
				= CommonBasicValues::levelupPropertiesInc - attackInc - defenseInc;

			this->m_property.m_hpoints 
				+= _Random(static_cast<Value>((double)this->m_property.m_hpoints / 15.0)) + CommonBasicValues::hpointsInc;
		}
		break;

		case Career::Type::Yodian:
		{
			attackInc 
				= _Random(static_cast<Value>(CommonBasicValues::levelupPropertiesInc / 3), CommonBasicValues::levelupPropertiesInc);
			agilityInc 
				= _Random(CommonBasicValues::levelupPropertiesInc - attackInc);
			defenseInc 
				= CommonBasicValues::levelupPropertiesInc - attackInc - agilityInc;
			attackInc 
				+= ConvertValueByPercent(attackInc, Career::Yodian::damageDecIndex);	// special
			agilityInc 
				+= ConvertValueByPercent(agilityInc, Career::Yodian::agilityIncIndex);

			this->m_property.m_hpoints 
				+= CommonBasicValues::hpointsInc - _Random(static_cast<Value>((double)this->m_property.m_hpoints / 20.0));
		}
		break;

		case Career::Type::Michelle:
		{
			attackInc = _Random(static_cast<Value>(CommonBasicValues::levelupPropertiesInc / 3), static_cast<Value>(CommonBasicValues::levelupPropertiesInc / 2));
			defenseInc = _Random(CommonBasicValues::levelupPropertiesInc - attackInc);
			agilityInc = CommonBasicValues::levelupPropertiesInc - attackInc - defenseInc;
			attackInc += ConvertValueByPercent(attackInc, Career::Michelle::damageIncIndex);
			defenseInc += ConvertValueByPercent(defenseInc, Career::Michelle::defenseIncIndex);

			this->m_property.m_hpoints += static_cast<Value>(std::sqrt(std::sqrt(_Random(static_cast<Value>(CommonBasicValues::hpointsInc / 2), CommonBasicValues::hpointsInc)) * CommonBasicValues::hpointsInc) * PI);
		}
		break;

		default:
			break;
		}
		this->m_property.m_attack += attackInc;
		this->m_property.m_defense += defenseInc;
		this->m_property.m_agility += agilityInc;
	}

	void Assassin::_InitSkill_()
	{
		this->m_skill.tearingChance = +30;
		this->m_skill.slowChance = +10;
		this->m_skill.slowIndex = +200;
		this->m_skill.fastenIndex = +50;
		this->m_skill.stolenIndex = +50;
	}
}
