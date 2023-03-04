using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using MyLibs;

namespace OpenAiChat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                answers();
                request.Clear();
            }
        }
        async Task<IAsyncResult> answers()
        {
            string apiKey = " "; //your apikey
            // адрес api для взаимодействия с чат-ботом
            string endpoint = "https://api.openai.com/v1/chat/completions";
            // набор соообщений диалога с чат-ботом
            List<Message> messages = new List<Message>();
            // HttpClient для отправки сообщений
            var httpClient = new HttpClient();

            while (true)
            {
                // ввод сообщения пользователя
                var content = request.Text;
                
                // если введенное сообщение имеет длину меньше 1 символа
                // то выходим из цикла и завершаем программу
                if (content is not { Length: > 0 }) break;
                answer.Text += ($"\n You: {content}");
                // формируем отправляемое сообщение
                var message = new Message() { Role = "user", Content = content };
                // добавляем сообщение в список сообщений
                messages.Add(message);

                // формируем отправляемые данные
                var requestData = new Request()
                {
                    ModelId = "gpt-3.5-turbo",
                    Messages = messages
                };

                // устанавливаем отправляемый в запросе токен
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                // отправляем запрос
                using var response = await httpClient.PostAsJsonAsync(endpoint, requestData);

                // если произошла ошибка, выводим сообщение об ошибке на консоль
                if (!response.IsSuccessStatusCode)
                {
                    answer.Text = ($"{(int)response.StatusCode} {response.StatusCode}");
                    break;
                }

                // получаем данные ответа
                ResponseData? responseData = await response.Content.ReadFromJsonAsync<ResponseData>();

                var choices = responseData?.Choices ?? new List<Choice>();
                if (choices.Count == 0)
                {
                    answer.Text = ("No choices were returned by the API");
                    continue;
                }

                var choice = choices[0];
                var responseMessage = choice.Message;
                // добавляем полученное сообщение в список сообщений
                messages.Add(responseMessage);
                var responseText = responseMessage.Content.Trim();
                answer.Text += ($"\n ChatGPT: {responseText}");
            }
            return null;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Скопируй сам");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
