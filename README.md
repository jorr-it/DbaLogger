# DbaLogger
WPF application for storing dBA measurements

This Windows desktop application reads the raw data stream from the sound level meter DT-8851 and DT-8852 also known as Voltcraft SL-400 and stores it as a dBA measurement per second in a MySQL table.

The application is higly based on the source code of this project by Leif Simon Goodwin:
https://www.codeproject.com/Articles/1173686/A-Csharp-System-Tray-Application-using-WPF-Forms

Description of the interface protocol of the sound level meter in PDF:
http://www.produktinfo.conrad.com/datenblaetter/100000-124999/100680-da-01-en-Schnittstellenbeschr_Schallpegelm_SL_300.pdf

Written .NET version 4.6.1 using Entity Framework 6 with a Code First connection of MySQL.
