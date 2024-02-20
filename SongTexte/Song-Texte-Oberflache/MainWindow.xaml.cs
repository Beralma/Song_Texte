using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Models;

using Microsoft.Web.WebView2.Core;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.Win32;

namespace Song_Texte_Oberflache
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Chat
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Chat(string content)
        {
            Content = content;
        }
    }

    internal class CompletionCreateRequest : CompletionRequest
    {
        public string Prompt { get; set; }
        public string Model { get; set; }
        public double Temperature { get; set; }
        public int MaxTokens { get; set; }
    }

    public partial class MainWindow : Window
    {
        OpenAIAPI openAiClient = new OpenAIAPI("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");

        public void DisplayResults(IEnumerable<Chat> results)
        {
            ChatBox.ItemsSource = results;
        }

        public async Task<Chat> QueryChatGPT(string promt)
        {
            try
            {
                var completion = await openAiClient.Completions.CreateCompletionAsync(
                    new CompletionCreateRequest
                    {
                        Prompt = promt,
                        Model = Model.ChatGPTTurbo,
                        Temperature = 0.1,
                        MaxTokens = 1000
                    });

                // Save the result in the database
                var db = new SQLiteConnection(new SQLitePlatformWin32(), "Song_Texte.db");
                db.CreateTable<Chat>();
                var chat = new Chat(completion.Choices[0].Text);
                db.Insert(chat);

                return chat;
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task DisplayChatHistoryAsync()
        {
            try
            {
                var db = new SQLiteConnection(new SQLitePlatformWin32(), "Song_Texte.db");
                var results = db.Table<Chat>().OrderByDescending(c => c.Id).ToList();
                DisplayResults(results);
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine(ex.Message);
            }
        }

        public void AddToChatBox(Chat chat)
        {
            if (chat != null)
            {
                ChatBox.Items.Add(chat);
            }
        }

        public void DeleteChat(Chat chat)
        {
            var db = new SQLiteConnection(new SQLitePlatformWin32(), "Song_Texte.db");
            db.Delete(chat);
            ChatBox.Items.Remove(ChatBox.Items.Cast<Chat>().FirstOrDefault(c => c.Id == chat.Id));
        }

        public async Task<string> FindCategory(string theme, string ent)
        {
            string catQuery = "Create song lyrics with the following theme and in style of the entrepreneur: " + theme + " " + ent;

            string category = await QueryChatGPT(catQuery);

            return category?.ToLower();
        }

        private async void querybox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var chat = new Chat(querybox.Text);
                AddToChatBox(chat);

                string[] inputs = querybox.Text.Split(' ');
                if (inputs.Length >= 2)
                {
                    string category = await FindCategory(inputs[0], inputs[1]);

                    chat.Content = category;
                    var result = await QueryChatGPT(querybox.Text);
                    AddToChatBox(result);
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Connect to the database or create it if it doesn't exist
            var db = new SQLiteConnection(new SQLitePlatformWin32(), "Song_Texte.db");
            db.CreateTable<Chat>();
        }

    }
}