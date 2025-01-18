Algorithm Benchmark Project

Project Overview

The Algorithm Benchmark Project is designed to analyze the performance of various sorting algorithms, including Heap Sort, Radix Sort, Shell Sort, and Quick Sort. The analysis is conducted on arrays of sizes 1K, 10K, and 100K, with three different types: random, reverse, and partially sorted arrays. The project evaluates CPU usage, RAM usage, and execution time for each case.

Features

Index Page: Allows users to select the sorting algorithm, array size, and array type to benchmark.

Result Page: Displays detailed results, including performance metrics of the selected algorithm.

Report Page: Generates a summary report based on previously executed algorithms, with an option to export the report as a PDF.

Technologies Used

Backend: .NET 8, Entity Framework

Frontend: Bootstrap, HTML, CSS, JavaScript, ApexCharts.js

Database: MySQL

Prerequisites

To run the project, ensure you have the following:

MySQL

phpMyAdmin

.NET 8

Visual Studio

Entity Framework (MySQL connector)

Installation Steps

Set Up the Database:

Create a new database named algo in your MySQL server.

Import the algo.sql file located in the project’s root directory into the algo database.

Open the Project:

Open the project in Visual Studio.

Install the required NuGet packages as specified in the project dependencies.

Configure Database Connection:

If necessary, update the database connection details in the appsettings.json file.

Run the Project:

Start the application from Visual Studio to launch the project.

Usage Guide

Benchmarking Algorithms:

Navigate to the Index Page to select the desired algorithm, array size, and type.

Click "Run" to execute the selected benchmark.

Viewing Results:

Check the Result Page for detailed performance metrics of the executed algorithm.

Generating Reports:

Go to the Report Page to view summaries of previously executed benchmarks.

Optionally, export the report as a PDF.

Project Structure

Index Page: User interface for selecting benchmark options.

Result Page: Displays benchmark results, including CPU, RAM, and execution time.

Report Page: Summarizes past benchmark logs with an export option.
