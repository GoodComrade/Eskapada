using System;
using System.Linq;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class AnaliticsController : MonoBehaviour
{
	public List<GameObject> screens = new List<GameObject> ();

    public InputField ui_ДатаС;
    public InputField ui_ДатаПо;

    public Dropdown ТипУстройстваDropDown;

    [Space(5)]

    public GameObject GraphHolder;

    public Image GraphImage;

    public Color GraphBackground;
    public Color GraphColor;

    public List<Text> GraphDateValues = new List<Text>();
    public Text GraphMaxValue;

    public Vector2Int GraphRect = new Vector2Int(800, 600);

    private Color GraphImageStartColor;

    private List<GraphData> GraphDatas = new List<GraphData>();
    

    private void Awake()
    {
        Requester4net.OnDataResponse += OnGetDeviceData;

        GraphImageStartColor = GraphImage.color;

        GraphDateValues.ForEach(x =>
        {
            x.gameObject.SetActive(false);
        });

        GraphMaxValue.gameObject.SetActive(false);

        GraphHolder.SetActive(false);

        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
        optionDatas.Add(new Dropdown.OptionData("Холодная вода"));
        optionDatas.Add(new Dropdown.OptionData("Горячая вода"));
        optionDatas.Add(new Dropdown.OptionData("Электричество"));
        optionDatas.Add(new Dropdown.OptionData("Газ"));
        optionDatas.Add(new Dropdown.OptionData(" "));

        ТипУстройстваDropDown.AddOptions(optionDatas);
    }

    private void OnGetDeviceData()
    {
        if (ui_ДатаПо.text.Length == 10 && ui_ДатаС.text.Length == 10)
        {
            DateTime dateFrom = DateTime.ParseExact(ui_ДатаС.text, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime dateTo = DateTime.ParseExact(ui_ДатаПо.text, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            
            var типУстройства = TypeChannel.unknown;

            switch (ТипУстройстваDropDown.value)
            {
                case 0:
                    типУстройства = TypeChannel.ХолоднаяВода;
                    break;
                case 1:
                    типУстройства = TypeChannel.ГорячаяВода;
                    break;
                case 2:
                    типУстройства = TypeChannel.Электросчетчик;
                    break;
                case 3:
                    типУстройства = TypeChannel.Газ;
                    break;
            }

            //var нужноеУстройство = UserData.Devices
            //    .Where(x => x.channels.Any(y => y.GetTypeChannel == типУстройства))
            //    .FirstOrDefault();

            //if (нужноеУстройство != null)
            //{
            //    var история = нужноеУстройство.channels
            //        .Where(x => x.GetTypeChannel == типУстройства)
            //        .FirstOrDefault()
            //        .history;

            //    Debug.Log("<color=#ff00ff>Найдено устройство " + типУстройства.ToString("G") + ". Истории " + история.Count + "</color>");

            //    история.ForEach(x =>
            //        {
            //            Debug.Log("<color=#ff00ff>Найдено устройство " + типУстройства.ToString("G") +
            //                "! Дата информации: " + UserData.IntToDate(x.time) +
            //                "</color>");
            //        });
            //}
            //else
            //{
            //    Debug.Log("<color=#ff00ff>Устройство " + типУстройства.ToString("G") + " не найдено!</color>");
            //}

            GraphDatas = UserData.ПолучитьИнформациюУстройствЗаВремя(типУстройства, dateFrom, dateTo);
        }

    }

    private void Update()
    {
        if (GraphDatas.Count > 0)
        {
            if (GraphDatas.Count > 1)
            {
                var graphValues = GraphDatas.Select(x => x.Value).ToArray();

                GraphImage.color = Color.white;
                GraphImage.sprite = SpriteCreator.Create(graphValues, GraphBackground, GraphColor, GraphRect);

                int startDate = UserData.DateToInt(GraphDatas[0].Date);
                int endDate = UserData.DateToInt(GraphDatas[GraphDatas.Count - 1].Date);
                int delta = (endDate - startDate) / GraphDateValues.Count;

                if (GraphDateValues.Count == GraphDatas.Count)
                {
                    for (int i = 0; i < GraphDateValues.Count; i++)
                    {
                        GraphDateValues[i].text = GraphDatas[i].Date.ToString("dd.MM");
                    }
                }
                else
                {
                    for (int i = 0; i < GraphDateValues.Count; i++)
                    {
                        int dateInt = delta * i + startDate;
                        DateTime date = UserData.IntToDate(dateInt);
                        GraphDateValues[i].text = date.ToString("dd.MM");
                    }
                }

                GraphMaxValue.text = graphValues.Max().ToString();

                GraphDateValues.ForEach(x =>
                {
                    x.gameObject.SetActive(true);
                });

                GraphMaxValue.gameObject.SetActive(true);
                GraphHolder.SetActive(true);
            }
            
            GraphDatas.Clear();
        }
    }

    private void OnDestroy()
    {
        Requester4net.OnDataResponse -= OnGetDeviceData;
    }

    public void BackScreen()
    {
		screens [0].SetActive (true);
		screens [1].SetActive (false);
	}

    public void OnInputValueChangedДатаС()
    {
        GetDateStringFromString(ui_ДатаС);
        ПроверитьДаты();
    }

    public void OnInputValueChangedДатаПо()
    {
        GetDateStringFromString(ui_ДатаПо);
        ПроверитьДаты();
    }

    public void ПроверитьДаты()
    {
        if (ui_ДатаПо.text.Length == 10 && ui_ДатаС.text.Length == 10)
        {
            try
            {
                DateTime dateFrom = DateTime.ParseExact(ui_ДатаС.text, "dd.MM.yyyy", CultureInfo.InvariantCulture) + new TimeSpan(2, 0, 0, 0);
                DateTime dateTo = DateTime.ParseExact(ui_ДатаПо.text, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                if (dateTo < dateFrom)
                {
                    dateTo = dateFrom;
                }

                ui_ДатаПо.text = dateTo.ToString("dd.MM.yyyy");

                dateFrom = DateTime.ParseExact(ui_ДатаС.text, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                Requester4net.ПолучитьИнформациюОВсехУстройствах(dateFrom, dateTo);

                GraphImage.sprite = null;
                GraphImage.color = GraphImageStartColor;
                GraphDateValues.ForEach(x =>
                {
                    x.gameObject.SetActive(false);
                });

                GraphMaxValue.gameObject.SetActive(false);
                GraphHolder.SetActive(false);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Не удалось распознать даты: " + exception.Message);
            }
        }
    }

    private string GetDateStringFromString(InputField input)
    {
        string value = input.text;
        string result = string.Empty;

        // Проверка символов на число и соединение точками.

        foreach (var symbol in value)
        {
            if (char.IsDigit(symbol))
            {
                if (result.Length == 2 || result.Length == 5)
                {
                    result += ".";
                }

                result += symbol;

                if (result.Length == 10) // Дата dd.MM.yyyy - 10 символов
                {
                    break;
                }
            }
        }

        // Проверка года: только больше 2018.

        var dateArray = result.Split('.');

        if (dateArray.Length == 3 && result.Length == 10)
        {
            int year = 0;
            if (int.TryParse(dateArray[2], out year))
            {
                if (year < 2018)
                {
                    dateArray[2] = "2018";
                }
            }
        }

        // Проверка месяца: не больше 12.

        if (dateArray.Length > 1)
        {
            int месяц = 0;
            if (int.TryParse(dateArray[1], out месяц))
            {
                if (месяц > 12)
                {
                    dateArray[1] = "12";
                }
            }
        }

        result = string.Join(".", dateArray);

        // Изменяем место ввода текста в конец.

        input.selectionAnchorPosition = result.Length;
        input.selectionFocusPosition = result.Length;

        input.text = result;

        return result;
    }
}

public class GraphData
{
    public float Value;
    public DateTime Date;
}
