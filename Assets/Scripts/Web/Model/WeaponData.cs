using System.Collections.Generic;

namespace WebData
{
    public class GameRounds
    {
        public List<GameRound> rounds;
    }

    public class GameRound
    {
        int roundId;
        public List<PlayerRoundData> playersRounds; // [0] - игрок, [1] - противник (относительно того, кто отправляет запрос)
        public int winner; // -1 - нет победителя, 0 - игрок, 1 - противник (относительно того, кто отправляет запрос)
    }

    public class PlayerRoundData
    {
        public int currentHealth;
        public int maxHealth;
        public int damage; // урон, нанесенный игроком за ход. -1 если ход не сделан
        public WeaponData weapon; // использованное в данном ходе оружие (может быть null, если ход не сделан)
        public List<WeaponData> awailableWeapons; // оружие, доступное игроку в данном ходе
        public List<EffectData> effectsData; // список наложенных игроком эффектов (можеть быть null если эффектов нет)
    }

    public class WeaponData
    {
        public int weaponId;
        public string description;
        public int rarity; // чем больше, тем выше редкость
        public double minDamage;
        public double maxDamage;
        public int durability;
        public List<EffectData> possiblePerks;
    }
    public class EffectData
    {
        public int perkId;
        public string description;
        public string type; // тип перка (блок оружия, урон за ход итд). Нужно расписать все возможные
        public List<double> parameters; // параметры для каждого перка считываются по своему паттерну.
                                        // например для блока оружия [0] - количество ходов, [1] и далее - id заблокированного оружия
                                        // расписать порядок данных для каждого перка
    }
}


