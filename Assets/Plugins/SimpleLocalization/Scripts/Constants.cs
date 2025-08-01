using System.Collections.Generic;

namespace Assets.SimpleLocalization.Scripts
{
    public static class Constants
    {
        public const string LocalizationEditorUrl = "https://script.google.com/macros/s/AKfycbz8pTVnDTeLga3hr2tFRc2W1YdsFQPzAtRCOD3fpCe6HoMAZIeD9e_Zauwek1a5zndOHA/exec";//макрос выгрузки таблиц 
        public const string SheetResolverUrl = "https://script.google.com/macros/s/AKfycbycW2dsGZhc2xJh2Fs8yu9KUEqdM-ssOiK1AlES3crLqQa1lkDrI4mZgP7sJhmFlGAD/exec";//макрос импорта таблиц
        public const string TableUrlPattern = "https://docs.google.com/spreadsheets/d/{0}";
        public const string ExampleTableId = "1RvKY3VE_y5FPhEECCa5dv4F7REJ7rBtGzQg9Z_B_DE4";//ссылка на тестовую таблицу
        public static readonly Dictionary<string, int> ExampleSheets = new();//библиотека всех страниц
    }
}