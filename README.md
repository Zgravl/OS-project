# OS Project 1

## Overview

This repository contains two projects demonstrating multi-threading and IPC using C#:

1. **Project A: Multi-Threading and Deadlock Resolution**  
   - Implements a banking system with concurrent deposits and withdrawals.
   - Uses locking mechanisms to prevent race conditions.
   - Demonstrates deadlock detection and resolution.
   - Includes parallel computation using multiple threads.

2. **Project B: Inter-Process Communication (IPC) with Named Pipes**  
   - Implements a producer-consumer model using named pipes.
   - The producer sends data to the consumer via an IPC mechanism.
   - The consumer reads and processes the data in real time.

---

## Requirements and Dependencies

Ensure you have the following installed before running the projects:

- **.NET 8 SDK** (Required for building and running C# applications)  
  [Download .NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

- **Windows Subsystem for Linux (WSL)** (Optional, if using Linux through WSL)

To check if you have .NET installed, run:

```sh
dotnet --version
```

## Environment Setup

This project was tested using Windows 10 though should also work using windows 11
- to install wsl open a command prompt and type
  ```
wsl --install
```

