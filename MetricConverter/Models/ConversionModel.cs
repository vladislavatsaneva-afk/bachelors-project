using System;
using System.Collections.Generic;

namespace MetricConverter.Models
{
    public class ConversionModel
    {
        public string ConversionType { get; set; }
        public double Value { get; set; }
        public string FromUnit { get; set; }
        public string ToUnit { get; set; }
        public double Result { get; set; }      
        public string WarningMessage { get; set; }
        public List<ConversionHistoryEntry> History { get; set; }

        public double Convert()
        {
            WarningMessage = null;
            try
            {
                if (ConversionType != "Температура" && Value < 0)
                {
                    WarningMessage = "Отрицателни стойности са позволени САМО при преобразуване на температура.";
                    Result = 0;
                    return Result;
                }

                switch (ConversionType)
                {
                    case "Разстояние":
                        Result = ConvertDistance(Value, FromUnit, ToUnit);
                        break;
                    case "Тегло":
                        Result = ConvertWeight(Value, FromUnit, ToUnit);
                        break;
                    case "Температура":
                        Result = ConvertTemperature(Value, FromUnit, ToUnit);
                        break;
                    case "Обем":
                        Result = ConvertVolume(Value, FromUnit, ToUnit);
                        break;
                    case "Скорост":
                        Result = ConvertVelocity(Value, FromUnit, ToUnit);
                        break;
                    case "Време":
                        Result = ConvertTime(Value, FromUnit, ToUnit);
                        break;
                    case "Валута":
                        Result = ConvertCurrency(Value, FromUnit, ToUnit);
                        break;
                    case "Площ":
                        Result = ConvertArea(Value, FromUnit, ToUnit);
                        break;
                    case "Енергия":
                        Result = ConvertEnergy(Value, FromUnit, ToUnit);
                        break;
                    case "Мощност":
                        Result = ConvertPower(Value, FromUnit, ToUnit);
                        break;
                    case "Налягане":
                        Result = ConvertPressure(Value, FromUnit, ToUnit);
                        break;
                    case "Сила":
                        Result = ConvertForce(Value, FromUnit, ToUnit);
                        break;
                    case "Честота":
                        Result = ConvertFrequency(Value, FromUnit, ToUnit);
                        break;
                    case "Информация":
                        Result = ConvertData(Value, FromUnit, ToUnit);
                        break;
                    case "Светлина":
                        Result = ConvertLightDistance(Value, FromUnit, ToUnit);
                        break;
                    default:
                        WarningMessage = "Неподдържано преобразувание.";
                        Result = 0;
                        break;
                }
            }
            catch (Exception ex)
            {
                WarningMessage = $"Съжалявам: {ex.Message}";
                Result = 0;
            }
            return Result;
        }

        private double ConvertDistance(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Миля", "Сантиметър"), 160934},
                {("Миля", "Метър"), 1609.34},
                {("Миля", "Километър"), 1.60934},
                {("Миля", "Милиметър"), 1.609e+6},
                {("Милиметър", "Миля"), 6.2137e-7},
                {("Милиметър", "Сантиметър"), 0.1},
                {("Милиметър", "Метър"), 0.001},
                {("Милиметър", "Километър"), 1e-6},
                {("Сантиметър", "Миля"), 0.00000621371},
                {("Сантиметър", "Милиметър"), 10},
                {("Сантиметър", "Метър"), 0.01},
                {("Сантиметър", "Километър"), 0.00001},
                {("Метър", "Миля"), 0.000621371},
                {("Метър", "Милиметър"), 1000},
                {("Метър", "Сантиметър"), 100},
                {("Метър", "Километър"), 0.001},
                {("Километър", "Миля"), 0.621371},
                {("Километър", "Милиметър"), 1e+6},
                {("Километър", "Метър"), 1000},
                {("Километър", "Сантиметър"), 100000}               
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }

        private double ConvertWeight(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Фунт", "Милиграм"), 453592},
                {("Фунт", "Грам"), 453.592},
                {("Фунт", "Килограм"), 0.453592},
                {("Фунт", "Тон"), 0.000453592},
                {("Милиграм", "Фунт"), 2.2046e-6},
                {("Милиграм", "Грам"), 0.001},
                {("Милиграм", "Килограм"), 1e-6},
                {("Милиграм", "Тон"), 1e-9},
                {("Грам", "Фунт"), 0.00220462},
                {("Грам", "Милиграм"), 1000},
                {("Грам", "Килограм"), 0.001},
                {("Грам", "Тон"), 1e-6},
                {("Килограм", "Фунт"), 2.20462},
                {("Килограм", "Милиграм"), 1e+6},
                {("Килограм", "Грам"), 1000},
                {("Килограм", "Тон"), 0.001},
                {("Тон", "Фунт"), 2204.62},
                {("Тон", "Милиграм"), 1e+9},
                {("Тон", "Грам"), 1e+6},
                {("Тон", "Килограм"), 1000}
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }

        private double ConvertTemperature(double value, string fromUnit, string toUnit)
        {
            if (fromUnit == toUnit) return value;

            return (fromUnit, toUnit) switch
            {
                ("Целзий", "Келвин") => value + 273.15,
                ("Целзий", "Фаренхайт") => (value * 9 / 5) + 32,
                ("Келвин", "Целзий") => value - 273.15,
                ("Келвин", "Фаренхайт") => (value - 273.15) * 9 / 5 + 32,
                ("Фаренхайт", "Целзий") => (value - 32) * 5 / 9,
                ("Фаренхайт", "Келвин") => (value - 32) * 5 / 9 + 273.15,
                _ => throw new InvalidOperationException("Неподдържано преобразувание.")
            };
        }

        private double ConvertVolume(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Унции", "Милилитър"), 29.5735},
                {("Унции", "Литър"), 0.0295735},
                {("Унции", "Килолитър"), 2.95735e-4},
                {("Милилитър", "Унции"), 0.033814},
                {("Милилитър", "Литър"), 0.001},
                {("Милилитър", "Килолитър"), 1e-6},
                {("Литър", "Унции"), 33.814},
                {("Литър", "Милилитър"), 1000},
                {("Литър", "Килолитър"), 0.001},
                {("Килолитър", "Унции"), 33814.0},
                {("Килолитър", "Милилитър"), 1e+6},
                {("Килолитър", "Литър"), 1000},
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }

        private double ConvertVelocity(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                { ("Миля в час", "Километър в час"), 1.60934 },
                { ("Миля в час", "Метър в секунда"), 0.44704 },
                { ("Километър в час", "Миля в час"), 0.621371 },
                { ("Километър в час", "Метър в секунда"), 0.277778 },
                { ("Метър в секунда", "Миля в час"), 2.23694 },
                { ("Метър в секунда", "Километър в час"), 3.6 }               
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }

        private double ConvertTime(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Секунда", "Минута"), 1.0 / 60},
                {("Секунда", "Час"), 1.0 / 3600},
                {("Минута", "Секунда"), 60},
                {("Минута", "Час"), 1.0 / 60},
                {("Минута", "Ден"), 1.0 / 1440},
                {("Минута", "Седмица"), 1.0 / 10080},
                {("Час", "Секунда"), 3600},
                {("Час", "Минута"), 60},
                {("Час", "Ден"), 1.0 / 24},
                {("Час", "Седмица"), 1.0 / 168},
                {("Ден", "Минута"), 1440},
                {("Ден", "Час"), 24},
                {("Ден", "Месец"), 1.0 / 30},
                {("Ден", "Година"), 1.0 / 365},
                {("Седмица", "Минута"), 10080},
                {("Седмица", "Час"), 168},
                {("Седмица", "Месец"), 1.0 / 4.34524},
                {("Седмица", "Година"), 1.0 / 52.1775},                
                {("Месец", "Ден"), 30},
                {("Месец", "Седмица"), 4.34524},
                {("Месец", "Година"), 1.0 / 12},
                {("Година", "Ден"), 365},
                {("Година", "Седмица"), 52.1775},
                {("Година", "Месец"), 12}
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }

        private double ConvertCurrency(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                { ("Лев", "Евро"), 0.51 },
                { ("Лев", "Долар"), 0.56 },
                { ("Лев", "Лира"), 0.44 },
                { ("Евро", "Лев"), 1.0 / 0.51 },
                { ("Евро", "Долар"), 1.09 },
                { ("Евро", "Лира"), 0.85 },
                { ("Долар", "Лев"), 1.0 / 0.56 },
                { ("Долар", "Евро"), 1.0 / 1.09 },
                { ("Долар", "Лира"), 0.78 },
                { ("Лира", "Лев"), 1.0 / 0.44 },
                { ("Лира", "Евро"), 1.0 / 0.85 },
                { ("Лира", "Долар"), 1.0 / 0.78 }          
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }
        private double ConvertArea(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Квадратен метър", "Квадратен километър"), 0.000001},
                {("Квадратен метър", "Декар"), 0.001},
                {("Квадратен метър", "Хектар"), 0.0001},
                {("Квадратен километър", "Квадратен метър"), 1_000_000},
                {("Квадратен километър", "Декар"), 1000},
                {("Квадратен километър", "Хектар"), 100},
                {("Декар", "Квадратен метър"), 1000},
                {("Декар", "Квадратен километър"), 0.001},
                {("Декар", "Хектар"), 0.1},
                {("Хектар", "Квадратен метър"), 10_000},
                {("Хектар", "Квадратен километър"), 0.01},
                {("Хектар", "Декар"), 10}
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }

        private double ConvertEnergy(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Калория", "Килокалория"), 0.001},
                {("Калория", "Килоджаул"), 0.004184},
                {("Килокалория", "Калория"), 1000},
                {("Килокалория", "Килоджаул"), 4.184},
                {("Килоджаул", "Калория"), 239.005736},               
                {("Килоджаул", "Килокалория"), 0.239005736}
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }

        private double ConvertPower(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Ват", "Киловат"), 0.001},
                {("Ват", "Мегават"), 0.000001},
                {("Ват", "Гигават"), 0.000000001},
                {("Киловат", "Ват"), 1000},
                {("Киловат", "Мегават"), 0.001},
                {("Киловат", "Гигават"), 0.000001},
                {("Мегават", "Ват"), 1_000_000},
                {("Мегават", "Киловат"), 1000},
                {("Мегават", "Гигават"), 0.001},
                {("Гигават", "Ват"), 1_000_000_000},
                {("Гигават", "Киловат"), 1_000_000},
                {("Гигават", "Мегават"), 1000}
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразуване.");
        }
        private double ConvertPressure(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Бар", "Атмосфера"), 0.986923},
                {("Атмосфера", "Бар"), 1.01325}
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }

        private double ConvertForce(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Нютон", "Килонютон"), 0.001},
                {("Нютон", "Понд"), 0.224809},
                {("Нютон", "Килопонд"), 0.1019716},
                {("Килонютон", "Нютон"), 1000},
                {("Килонютон", "Понд"), 224.809},
                {("Килонютон", "Килопонд"), 101.9716},
                {("Понд", "Нютон"), 4.44822},
                {("Понд", "Килонютон"), 0.00444822},
                {("Понд", "Килопонд"), 0.453592},
                {("Килопонд", "Нютон"), 9.80665},
                {("Килопонд", "Килонютон"), 0.00980665},
                {("Килопонд", "Понд"), 2.20462}
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }
        private double ConvertFrequency(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Херц", "Килохерц"), 0.001},
                {("Херц", "Мегахерц"), 0.000001},
                {("Херц", "Гигахерц"), 0.000000001},
                {("Килохерц", "Херц"), 1000},
                {("Килохерц", "Мегахерц"), 0.001},
                {("Килохерц", "Гигахерц"), 0.000001},
                {("Мегахерц", "Херц"), 1000000},
                {("Мегахерц", "Килохерц"), 1000},
                {("Мегахерц", "Гигахерц"), 0.001},
                {("Гигахерц", "Херц"), 1_000_000_000},
                {("Гигахерц", "Мегахерц"), 1000},             
                {("Гигахерц", "Килохерц"), 1_000_000}
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }
        private double ConvertData(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Бит", "Килобит"), 0.000125}, {("Килобит", "Бит"), 8000},
                {("Бит", "Мегабит"), 0.000000125}, {("Мегабит", "Бит"), 8000000},
                {("Бит", "Гигабит"), 0.000000000125}, {("Гигабит", "Бит"), 8000000000},
                {("Бит", "Терабайт"), 1.25e-13}, {("Терабайт", "Бит"), 8e12},
                {("Байт", "Килобит"), 0.001}, {("Килобит", "Байт"), 1000},
                {("Байт", "Мегабит"), 0.000001}, {("Мегабит", "Байт"), 1000000},
                {("Байт", "Гигабит"), 0.000000001}, {("Гигабит", "Байт"), 1000000000},
                {("Байт", "Терабайт"), 1e-12}, {("Терабайт", "Байт"), 1e12},
                {("Килобит", "Мегабит"), 0.001}, {("Мегабит", "Килобит"), 1000},
                {("Килобит", "Гигабит"), 0.000001}, {("Гигабит", "Килобит"), 1000000},
                {("Килобит", "Терабайт"), 1e-9}, {("Терабайт", "Килобит"), 1e9},
                {("Мегабит", "Гигабит"), 0.001}, {("Гигабит", "Мегабит"), 1000},
                {("Мегабит", "Терабайт"), 1e-6}, {("Терабайт", "Мегабит"), 1e6},
                {("Гигабит", "Терабайт"), 0.001}, {("Терабайт", "Гигабит"), 1000},
                {("Килобайт", "Мегабайт"), 0.001}, {("Мегабайт", "Килобайт"), 1000},
                {("Килобайт", "Гигабайт"), 0.000001}, {("Гигабайт", "Килобайт"), 1000000},
                {("Килобайт", "Терабайт"), 1e-9}, {("Терабайт", "Килобайт"), 1e9},
                {("Мегабайт", "Гигабайт"), 0.001}, {("Гигабайт", "Мегабайт"), 1000},
                {("Мегабайт", "Терабайт"), 1e-6}, {("Терабайт", "Мегабайт"), 1e6},
                {("Гигабайт", "Терабайт"), 0.001}, {("Терабайт", "Гигабайт"), 1000}
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }
        private double ConvertLightDistance(double value, string fromUnit, string toUnit)
        {
            var conversionRates = new Dictionary<(string, string), double>
            {
                {("Миля", "Парсек"), 5.214e-14},
                {("Миля", "Светлинна година"), 1.701e-13},
                {("Километър", "Парсек"), 3.262e-14},
                {("Километър", "Светлинна година"), 1.057e-13},
                {("Парсек", "Миля"), 1.917e+13},
                {("Парсек", "Километър"), 3.086e+13},
                {("Парсек", "Светлинна година"), 3.26156},
                {("Светлинна година", "Миля"), 5.879e+12},
                {("Светлинна година", "Километър"), 9.461e+12},
                {("Светлинна година", "Парсек"), 0.306601}
            };

            if (fromUnit == toUnit) return value;
            if (conversionRates.TryGetValue((fromUnit, toUnit), out var rate)) return value * rate;
            throw new InvalidOperationException("Неподдържано преобразувание.");
        }
    }
}
