using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Data.Common;
using System.Drawing;
using Unity.VisualScripting.Dependencies.Sqlite;

public class DBManager : MonoBehaviour
{
    private string dbUri = "URI=file:mydb.sqlite";
    private string SQL_COUNT_ELEMNTS = "SELECT count(*) FROM Coches;";
    private string SQL_CREATE_COCHES = "CREATE TABLE IF NOT EXISTS Coches" +
        "(Id INTEGER UNIQUE NOT NULL PRIMARY KEY" +
        ", Color TEXT NOT NULL" +
        ", Modelo TEXT DEFAULT 'Sandero'" +
        ", Marca INTEGER REFERENCES Marcas);";
    private string SQL_CREATE_MARCAS = "CREATE TABLE IF NOT EXISTS Marcas" +
        "(Id INTEGER UNIQUE NOT NULL PRIMARY KEY" +
        ", Nombre TEXT NOT NULL DEFAULT 'Dacia');";
    private string[] MARCAS = {"Dacia","Mercedes","Audi","Ferrari","Range","Bendi","Renault","Peugeot","Benly","Porsche"};
    private string[] MODELOS = {"Sandero","A4","Testarrosa","Q3","Q5","Velar","208","Clio","Megane","911"};
    private string[] COLORES = { "Negro", "Blanco", "Rojo", "Azul", "Rosa", "Morado", "Gris", "Marrón", "Verde", "Amarillo" };
    private int NUM_COCHES = 1000;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        IDbConnection dbConnection = OpenDatabase();
        InitializeDB(dbConnection);
        AddRandomData(dbConnection);
        //AddWrongData(dbConnection);
        SearchByMarca(dbConnection,"Audi");

        dbConnection.Close();
        Debug.Log("End");
    }

    private void SearchByMarca(IDbConnection dbConnection, string marca)
    {
        IDbCommand dbCmd = dbConnection.CreateCommand();
        dbCmd.CommandText = $"SELECT Id FROM Marcas WHERE Nombre='{marca}';";
        IDataReader reader = dbCmd.ExecuteReader();
        if (!reader.Read())
        {
            return;
        }
        int id_marca = reader.GetInt32(0);
        reader.Close();

        dbCmd.CommandText = $"SELECT * FROM Coches WHERE Marca='{id_marca}';";
        reader = dbCmd.ExecuteReader();
        string coches = "";
        while(reader.Read())
        {
            coches += $"{reader.GetInt32(0)}, {reader.GetString(1)}, {reader.GetString(2)}\n";
        }
        Debug.Log(coches);
    }

    private IDbConnection OpenDatabase()
    {
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "PRAGMA foreign_keys = ON";
        dbCommand.ExecuteNonQuery();
        return dbConnection;
    }

    private void InitializeDB(IDbConnection dbConnection)
    {
        IDbCommand dbCmd = dbConnection.CreateCommand();
        dbCmd.CommandText = SQL_CREATE_MARCAS + SQL_CREATE_COCHES;
        dbCmd.ExecuteReader();
    }

    private void AddWrongData(IDbConnection dbConnection)
    {
        string command = "INSERT INTO Coches (Color,Modelo,Marca) VALUES ";
        command += "('Rojo', '200', 201);";
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = command;
        dbCommand.ExecuteNonQuery();
    }

    private void AddRandomData(IDbConnection dbConnection)
    {
        if (CountNumberElements(dbConnection) > 0)
        {
            return;
        }
        int num_marcas = MARCAS.Length;
        string command = "INSERT INTO Marcas (Nombre) VALUES ";
        for (int i = 0; i < num_marcas; i++)
        {
            command += $"('{MARCAS[i]}'),";
        }
        command = command.Remove(command.Length - 1, 1);
        command += ";";
        command += "INSERT INTO Coches (Color,Modelo,Marca) VALUES ";
        System.Random rnd = new System.Random();
        for (int i = 0; i < NUM_COCHES; i++)
        {
            string color = COLORES[rnd.Next(COLORES.Length)];
            string modelo = MODELOS[rnd.Next(MODELOS.Length)];
            int marca = rnd.Next(num_marcas) + 1;
            command += $"('{color}','{modelo}','{marca}'),";
        }
        command = command.Remove(command.Length - 1, 1);
        command += ";";
        //Debug.Log(command);
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = command;
        dbCommand.ExecuteNonQuery();
    }

    private int CountNumberElements(IDbConnection dbConnection)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = SQL_COUNT_ELEMNTS;
        IDataReader reader = dbCommand.ExecuteReader();
        reader.Read();
        return reader.GetInt32(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
