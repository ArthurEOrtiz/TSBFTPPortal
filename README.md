# TSBFTPPortal Application

The TSBFTPPortal is a Windows Presentation Foundation (WPF) application designed for managing and interacting with FTP (File Transfer Protocol) services. 
The application features a TreeView interface that displays directories and files, with each item being represented by a `DirectoryItemViewModel`. 
Users can interact with this tree view to explore directories, and there is functionality to download files when selected. This README provides an overview of the application's features, architecture, and how to get started.

## Features

- **FTP File Management**: The application allows users to browse, explore, and manage files and directories hosted on FTP servers.

- **TreeView Interface**: It utilizes a TreeView user interface to display the directory structure and files, making it easy to navigate.

- **File Download**: Users can download files from the FTP server by selecting the desired file and initiating the download by double-clicking or hitting entere on the selected treeview item.

- **Search Functionality**: The application provides search capabilities, enabling users to filter items based on search criteria.

## Architecture

The TSBFTPPortal application follows the MVVM (Model-View-ViewModel) architectural pattern. It separates the user interface (View) from the application logic (ViewModel) and the underlying data (Model). Key architectural components include:
- **Models**: This represent the applications data. In this application `Counties` is the only model needed. 

- **ViewModels**: These represent the application's logic and state. 

- **Views**: These are responsible for the user interface. The primary View is the `TreeViewView` that displays the FTP directory structure.

- **Behaviors**: Custom behaviors like `DoubleClickBehavior` and  `EnterKeyBehavior` enhance user interaction.

- **Services**: These are responsible abstracting external dependencies, like the Crystal Reports Viewer and the Script Runner. And Handling asynchronous operations like the downloading and viewing files from the FTP Server

- **Themes**: This holds .xaml files that control the overal visual style of the application.

## Getting Started

Follow these steps to set up and run the TSBFTPPortal application:

1. **Prerequisites**: Ensure you have the following installed on your machine:
    - .NET Framework (Version 6)
    - Visual Studio 2022
    - Crystal Reports Run Time
    - Script Runner 

2. **Clone the Repository**: Clone this GitHub repository to your local machine.

3. **Open in Visual Studio**: Open the project in Visual Studio.

4. **Build and Run**: Build the project and run the application. You should see the TreeView interface for FTP file management.

5. **Exploring FTP**: Use the TreeView to navigate directories, download files, and explore the FTP server.

## Known Issues

- No Issues have been discovered yet. Note that this is still under develpment. 

## License

This project is licensed under the [MIT License](LICENSE.md).

---

_Arthur Edward Ortiz 2023-09-19_
