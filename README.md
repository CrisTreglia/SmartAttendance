Face Recognition with API Integration (C# & Python) - UNDER DEVELOPMENT
This project combines a C# API for student photo management with a Python-based real-time facial recognition system. The Python script fetches student photos from the C# API, which sources its data from a database, to enhance the recognition model dynamically.

Key Features
C# API with Database Integration: Manages and serves student photos from a database in SQL Server.
Real-time Face Detection: Identifies faces from a webcam feed using OpenCV.
API-Driven Model Updates: Fetches student photos from the C# API and dynamically incorporates them into the recognition model.
Error Handling: Includes robust error handling for API requests, database interactions, and file operations.
Requirements
C# Side:
.NET SDK
[ASP.NET Core]
[SQL Server]
Python Side:
Python (3.x recommended)
OpenCV (opencv-contrib-python)
requests library
