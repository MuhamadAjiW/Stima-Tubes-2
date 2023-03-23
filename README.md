# Maze Solver With BFS and DFS Algorithm
<!-- ## Table of Contents
* [General Info](#general-information)
* [Features](#features)
* [Screenshots](#screenshots)
* [Setup](#installation)
* [Usage](#program-execution)
* [Acknowledgements](#acknowledgements)

## General Information
This project is the 2nd project for algorithm strategy course in Bandung Institute of Technologi. In this project we were assigned to make a maze solver using breadth first search (BFS) and depth first search (DFS) algorithm implemented in C#. The maze is represented by a 2D array of char. The maze is solved by finding all  the treasure on the map. The start point is represented by K and the treasure is represented by T. The path is represented by R and the wall is represented by X. The program will output the path from the start point to all the treasure. The program will also output the number of nodes that are visited by the algorithm.

Authors:
| Name | NIM |
| --- | --- |
| Raditya Naufal A. | 13521022 |
| Muhammad Aji W. | 13521095 |
| Arsa Izdihar I. | 13521101 |

## Structure

```
.
|____README.md
|____.gitignore
|____bin
|____test
|____doc
|____src
| |____Spongbob.sln
| |____Spongbob
| | |____ViewLocator.cs
| | |____Models
| | | |____Graph.cs
| | | |____Algorithm.cs
| | | |____DFS.cs
| | | |____Parser.cs
| | | |____Map.cs
| | | |____Result.cs
| | | |____BFS.cs
| | |____App.axaml
| | |____Styles.axaml
| | |____app.manifest
| | |____App.axaml.cs
| | |____Spongbob.csproj
| | |____Views
| | | |____MainWindow.axaml
| | | |____ToggleButtonView.axaml.cs
| | | |____MainWindow.axaml.cs
| | | |____TileView.axaml.cs
| | | |____ToggleButtonView.axaml
| | | |____SidebarView.axaml.cs
| | | |____ResultView.axaml.cs
| | | |____ResultView.axaml
| | | |____SidebarView.axaml
| | | |____TileView.axaml
| | |____ViewModels
| | | |____ViewModelBase.cs
| | | |____EnumConverter.cs
| | | |____MainWindowViewModel.cs
| | | |____ToggleButtonViewModel.cs
| | | |____ResultViewModel.cs
| | | |____SidebarViewModel.cs
| | | |____TileViewModel.cs
| | |____Program.cs 
    
```

---

## How to Use

### Dependencies
- .NET framework (ver 6.0)
- Avalonia UI

### Installation

1. Clone repo using this command

```
git clone https://github.com/MuhamadAjiW/Tubes2_Spongbob.git
```

2. Run dotnet to build the project using this command

```
dotnet build
```

### Program Execution
1. (windows) Run Spongebob.exe in bin/Release/net6.0-windows
2. Choose desired map from test folder or anywhere in your device
3. Pick the configuration for BFS or DFS and TSP or Non-TSP
4. Press search or visualize button
5. Enjoy ^ ^

## Acknowledgements
- Bandung Institute of Technology, Informatics Engineering
- Munir, Rinaldi. (2022). IF2211 Strategi Algoritma - Semester II Tahun 2022/2023. Institut Teknologi Bandung. Diakses pada 23 Maret 2022, dari https://informatika.stei.itb.ac.id/~rinaldi.munir/Stmik/2022-2023/Tubes2-Stima-2023.pdf
