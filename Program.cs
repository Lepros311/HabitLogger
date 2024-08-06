﻿using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Globalization;

string? userMenuChoice;
bool closeApp = false;

string? habit;
string? unitOfMeasure;



do
{
    Console.WriteLine("What habit would you like to track?");
    habit = Console.ReadLine();
    if (habit != null)
    {
        habit = habit.Trim();
        habit = habit.ToLower();
        habit = habit.Replace(" ", "_");
    }
} while (habit == null);

do
{
    Console.WriteLine("What unit of measure would you like to use (e.g., glasses of water, hours of sleep, miles run, etc.)?");
    unitOfMeasure = Console.ReadLine();
} while (unitOfMeasure == null);

string connectionString = @"Data Source=HabitLogger.db";

using (var connection = new SQLiteConnection(connectionString))
{
    connection.Open();
    var tableCmd = connection.CreateCommand();

    tableCmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {habit} (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Date TEXT,
                                Quantity INTEGER
                                )"; 

    tableCmd.ExecuteNonQuery();

    connection.Close();
}

//Console.WriteLine("\nThese are the habits you currently have logged:");

//int habitNum = 1;

//using (var connection = new SQLiteConnection(connectionString))
//{
//    connection.Open();

//    using (SQLiteCommand command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table';", connection))

//    {

//        // Execute the command and read the results

//        using (SQLiteDataReader reader = command.ExecuteReader())

//        {

//            while (reader.Read())

//            {
//                if (reader["name"].ToString() == "sqlite_sequence")
//                {
//                    continue;
//                }
//                habit = reader["name"].ToString();
//                Console.WriteLine($"{habitNum}) {habit}");
//                habitNum++;
//            }

//        }

//    }
//}

//Console.WriteLine("\nEnter the number for the habit you want to track, or enter 0 to create a new habit:\n");

//string? habitChoice = Console.ReadLine();

// Function for user to select menu option

Console.WriteLine("\nThese are the habits you currently have logged:");

List<string> habitList = new List<string>();

int habitNum = 1;

try
{
    using (var connection = new SQLiteConnection(connectionString))
    {
        connection.Open();

        using (SQLiteCommand command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table';", connection))
        {
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string? habitName = reader["name"].ToString();

                    // Skip the sqlite_sequence table
                    if (habitName == "sqlite_sequence")
                    {
                        continue;
                    }

                    if (habitName != null)
                    {
                        habitList.Add(habitName);
                        Console.WriteLine($"{habitNum}) {habitName}");
                        habitNum++;
                    }
                    
                }
            }
        }
    }
}
catch (SQLiteException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\nEnter the number for the habit you want to track, or enter 0 to create a new habit:\n");

string? userInput = Console.ReadLine();

if (int.TryParse(userInput, out int habitIndex) && habitIndex > 0 && habitIndex <= habitList.Count)
{
    habit = habitList[habitIndex - 1];
}
else if (userInput == "0")
{
    Console.WriteLine("Create a new habit...");
    // Add code to create a new habit
}
else
{
    Console.WriteLine("Invalid input. Please try again.");
}


// Function to create a new habit



void UserSelectMenuOption()
{
    Console.WriteLine($"\nMAIN MENU - {habit}\n");
    Console.WriteLine("What would you like to do?\n");
    Console.WriteLine("Type 0 to Close Application.");
    Console.WriteLine("Type 1 to View All Records.");
    Console.WriteLine("Type 2 to Insert Record.");
    Console.WriteLine("Type 3 to Delete Record.");
    Console.WriteLine("Type 4 to Update Record.\n");
    string[] menuOptions = { "0", "1", "2", "3", "4" };
    do
    {
        userMenuChoice = Console.ReadLine();
        if (!menuOptions.Contains(userMenuChoice))
        {
            Console.WriteLine("That is not a valid option. You must enter 0, 1, 2, 3, or 4.");
        }
    } while (!menuOptions.Contains(userMenuChoice));
}

// Function to view all records (1)
void ViewAllRecords()
{
    using (var connection = new SQLiteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
            $"SELECT * FROM {habit}";

        List<DrinkingWater> tableData = new();

        SQLiteDataReader reader = tableCmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                tableData.Add(
                new DrinkingWater
                {
                    ID = reader.GetInt32(0),
                    Date = DateTime.ParseExact(reader.GetString(1), "MM-dd-yy", new CultureInfo("en-US")),
                    Quantity = reader.GetInt32(2)
                });
            }
        }
        else
        {
            Console.WriteLine("\nNo rows found");
        }

        connection.Close();

        Console.WriteLine("------------------------------------------------------------------");
        Console.WriteLine($"Habit Tracked: {habit}; Unit of Measure: {unitOfMeasure}");
        foreach (var dw in tableData)
        {
            Console.WriteLine($"{dw.ID} - {dw.Date.ToString("MMM-dd-yyyy")} - Quantity: {dw.Quantity}");
        }
        Console.WriteLine("------------------------------------------------------------------\n");
    }
}

// Function to insert record (2)
void InsertRecord()
{
    Console.WriteLine("Please insert the date: (Format: mm-dd-yy). Type 0 to return to main menu");

    string? dateInput = Console.ReadLine();

    while ((!DateTime.TryParseExact(dateInput, "MM-dd-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _)) && (dateInput != "0"))
    {
        Console.WriteLine("\nInvalid date. (Format: mm-dd-yy). Type 0 to return to main menu.\n");
        dateInput = Console.ReadLine();
    }

    if (dateInput == "0")
    {
        return;
    }

    string date = "";

    if (dateInput != null)
    {
        date = dateInput;
    }

    Console.WriteLine($"Please insert {unitOfMeasure} or other measure of your choice (no decimals allowed)");

    string? numberInput = Console.ReadLine();

    while (!Int32.TryParse(numberInput, out _) || (Convert.ToInt32(numberInput) < 0))
    {
        Console.WriteLine("\nInvalid number. Enter a whole number of 0 or greater.");
        numberInput = Console.ReadLine();
    }

    int finalInput = Convert.ToInt32(numberInput);

    int quantity = finalInput;

    using (var connection = new SQLiteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
            $"INSERT INTO {habit}(date, quantity) VALUES('{date}', {quantity})";

        tableCmd.ExecuteNonQuery();

        connection.Close();
    }
}

// Function to delete record (3)
void DeleteRecord()
{
    ViewAllRecords();

    Console.WriteLine("Please type the ID of the record you want to delete or type 0 to return to Main Menu\n");

    string? idInput = Console.ReadLine();

    if (idInput == "0")
    {
        return;
    }

    while (!Int32.TryParse(idInput, out _) || Convert.ToInt32(idInput) < 0)
    {
        Console.WriteLine("\nInvalid number, try again.\n");
        idInput = Console.ReadLine();
    }

    var recordID = Convert.ToInt32(idInput);

    using (var connection = new SQLiteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.CommandText = $"DELETE from {habit} WHERE ID = '{recordID}'";

        int rowCount = tableCmd.ExecuteNonQuery();

        if (rowCount == 0)
        {
            Console.WriteLine($"\nRecord with ID {recordID} doesn't exist.\n");
            DeleteRecord();
        }
    }

    Console.WriteLine($"\nRecord with ID {recordID} was deleted.\n");

    return;
}

// Function to update record (4)
void UpdateRecord()
{
    while (true)
    { 
        ViewAllRecords();

        Console.WriteLine("Please type the ID of the record you would like to update. Type 0 to return to Main Menu.");

        string? idInput = Console.ReadLine();

        if (idInput == "0")
        {
            return;
        }

        while (!Int32.TryParse(idInput, out _) || Convert.ToInt32(idInput) < 0)
        {
            Console.WriteLine("\nInvalid number, try again.\n");
            idInput = Console.ReadLine();
        }

        var recordID = Convert.ToInt32(idInput);

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM {habit} WHERE ID = {recordID})";
            int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (checkQuery == 0)
            {
                Console.WriteLine($"\nRecord with ID {recordID} doesn't exist.\n");
                continue;
            }

            Console.WriteLine("Please insert the date: (Format: mm-dd-yy). Type 0 to return to main menu");

            string? dateInput = Console.ReadLine();

            while ((!DateTime.TryParseExact(dateInput, "MM-dd-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _)) && (dateInput != "0"))
            {
                Console.WriteLine("\nInvalid date. (Format: mm-dd-yy). Type 0 to return to main menu.\n");
                dateInput = Console.ReadLine();
            }

            if (dateInput == "0")
            {
                return;
            }

            string date = "";

            if (dateInput != null)
            {
                date = dateInput;
            }

            Console.WriteLine($"Please insert {unitOfMeasure} or other measure of your choice (no decimals allowed)");

            string? numberInput = Console.ReadLine();

            while (!Int32.TryParse(numberInput, out _) || (Convert.ToInt32(numberInput) < 0))
            {
                Console.WriteLine("\nInvalid number. Enter a whole number of 0 or greater.");
                numberInput = Console.ReadLine();
            }

            int finalInput = Convert.ToInt32(numberInput);

            int quantity = finalInput;

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"UPDATE {habit} SET date = '{date}', quantity = {quantity} WHERE ID = {recordID}";

            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            tableCmd.ExecuteNonQuery();

            connection.Close();

            Console.WriteLine($"\nRecord with ID {recordID} was updated.");

            break;
        }
    }
}

// Habit Logger 

do
{
    UserSelectMenuOption();

    switch (userMenuChoice)
    {
        case "0":
            Console.WriteLine("Goodbye!");
            closeApp = true;
            Environment.Exit(0);
            break;
        case "1":
            ViewAllRecords();
            break;
        case "2":
            InsertRecord();
            break;
        case "3":
            DeleteRecord();
            break;
        case "4":
            UpdateRecord();
            break;
    }

} while (closeApp == false);


public class DrinkingWater
{
    public int ID { get; set; }

    public DateTime Date { get; set; }

    public int Quantity { get; set; }

}