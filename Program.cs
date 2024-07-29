string? userMenuChoice;
bool closeApp = false;

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

// Function to close application (0)
void CloseApplication()
{
    closeApp = true;
    Console.WriteLine("Goodbye!");
}

// Function to view all records (1)
void ViewAllRecords()
{
    Console.WriteLine("Here's all your records:");
}

// Function to insert record (2)
void InsertRecord()
{
    Console.WriteLine("Let's insert a record...");
}

// Function to delete record (3)
void DeleteRecord()
{
    Console.WriteLine("Let's delete a record...");
}

// Function to update record (4)
void UpdateRecord()
{
    Console.WriteLine("Let's update a record...");
}

// Habit Logger 

do
{
    UserSelectMenuOption();

    switch (userMenuChoice)
    {
        case "0":
            CloseApplication();
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