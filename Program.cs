using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Globalization;

string? userMenuChoice;
bool closeApp = false;

string connectionString = @"Data Source=HabitLogger.db";

using (var connection = new SQLiteConnection(connectionString))
{
    connection.Open();
    var tableCmd = connection.CreateCommand();

    tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Date TEXT,
                                Quantity INTEGER
                                )"; 

    tableCmd.ExecuteNonQuery();

    connection.Close();
}

// Function for user to select menu option
void UserSelectMenuOption()
{
    Console.WriteLine("\nMAIN MENU\n");
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
            $"SELECT * FROM drinking_water";

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
            Console.WriteLine("No rows found");
        }

        connection.Close();

        Console.WriteLine("-----------------------------------------------\n");
        foreach (var dw in tableData)
        {
            Console.WriteLine($"{dw.ID} - {dw.Date.ToString("MMM-dd-yyyy")} - Quantity: {dw.Quantity}");
        }
        Console.WriteLine("-----------------------------------------------\n");
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

    Console.WriteLine("Please insert number of glasses or other measure of your choice (no decimals allowed)");

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
            $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";

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

        tableCmd.CommandText = $"DELETE from drinking_water WHERE ID = '{recordID}'";

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
            checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE ID = {recordID})";
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

            Console.WriteLine("Please insert number of glasses or other measure of your choice (no decimals allowed)");

            string? numberInput = Console.ReadLine();

            while (!Int32.TryParse(numberInput, out _) || (Convert.ToInt32(numberInput) < 0))
            {
                Console.WriteLine("\nInvalid number. Enter a whole number of 0 or greater.");
                numberInput = Console.ReadLine();
            }

            int finalInput = Convert.ToInt32(numberInput);

            int quantity = finalInput;

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"UPDATE drinking_water SET date = '{date}', quantity = {quantity} WHERE ID = {recordID}";

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