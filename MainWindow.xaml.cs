using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;

namespace DailyPlanner
{
    public partial class MainWindow : Window
    {
        //Храним наши 'крутые' заметки в файле
        private ObservableCollection<Note> notes;
        private const string NotesFile = "notes.json";

        public MainWindow()
        {
            InitializeComponent();
            notes = LoadNotes();
            RefreshNotesListBox();
            datePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
        }
        //Показывает наши крутые заметки на выбранный день
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = datePicker.SelectedDate ?? DateTime.Today;
            notes = new ObservableCollection<Note>(LoadNotes().Where(n => n.Date.Date == selectedDate.Date));
            RefreshNotesListBox();
        }

        private ObservableCollection<Note> LoadNotes()
        {
            if (File.Exists(NotesFile))
            {
                using (var fs = new FileStream(NotesFile, FileMode.Open))
                {
                    var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Note>));
                    return (ObservableCollection<Note>)serializer.ReadObject(fs);
                }
            }
            return new ObservableCollection<Note>();
        }
        //Сохраняем заметку при первом создании
        private void SaveNotes()
        {
            using (var fs = new FileStream(NotesFile, FileMode.Create))
            {
                var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Note>));
                serializer.WriteObject(fs, notes);
            }
        }
        //Добавляем ещё не 'крутую' заметку :(
        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            var note = new Note
            {
                Title = "Новая заметка",
                Description = "Описание заметки",
                Date = datePicker.SelectedDate ?? DateTime.Today
            };
            notes.Add(note);
            SaveNotes();
            RefreshNotesListBox();
        }
        //Изменяем наши 'крутые' заметки 
        private void EditNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (notesListBox.SelectedItem is Note note)
            {
                editNoteStackPanel.Visibility = Visibility.Visible;
                editNoteStackPanel.DataContext = note;
            }
        }
        //Удаляем наши 'крутые' заметки
        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (notesListBox.SelectedItem is Note note)
            {
                notes.Remove(note);
                SaveNotes();
                RefreshNotesListBox();
            }
        }
        //Сохраняем изменения наших 'крутых' заметок
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNotes();
            editNoteStackPanel.Visibility = Visibility.Collapsed;
            RefreshNotesListBox();
        }
        //Обоновляем список заметок, если происходят изменения
        private void RefreshNotesListBox()
        {
            notesListBox.ItemsSource = null; 
            notesListBox.ItemsSource = notes;
            {
                notesListBox.SelectedIndex = 0;
            }
        }
    }
    //Тут создаём отдельный класс для заметок, где создаём для них свой тип данных и модельки
    public class Note
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}