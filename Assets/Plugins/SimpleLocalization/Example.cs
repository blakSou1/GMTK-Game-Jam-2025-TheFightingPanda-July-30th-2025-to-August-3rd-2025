using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleLocalization.Scripts;

namespace Assets.SimpleLocalization
{
	/// <summary>
	/// Asset usage example.
	/// </summary>
	public class Example : MonoBehaviour
	{
		//public Text FormattedText;

		/// <summary>
		/// Called on app start.
		/// </summary>
		public void Awake()
		{
			LocalizationManager.Read();

            LocalizationManager.Language = Application.systemLanguage switch
            {
                SystemLanguage.German => "German",
                SystemLanguage.Russian => "Russian",
                _ => "English",
            };//выбор системного языка при входе в игру

			/*
			// устанавливает перевод для таймера предавая текущее время (пример)
			FormattedText.text = LocalizationManager.Localize("Settings.Example.PlayTime", TimeSpan.FromHours(10.5f).TotalHours);

			// This way you can subscribe to LocalizationChanged event.
			LocalizationManager.OnLocalizationChanged += () => FormattedText.text = LocalizationManager.Localize("Settings.Example.PlayTime", TimeSpan.FromHours(10.5f).TotalHours);
			*/
		}

		/// <summary>
		/// Change localization at runtime.
		/// </summary>
		public void SetLocalization(string localization)
		{
			LocalizationManager.Language = localization;
		}//метод для ручного переключения языка
	}
}