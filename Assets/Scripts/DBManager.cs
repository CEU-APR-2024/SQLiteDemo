using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class DBManager : MonoBehaviour
{
    private string dbUri = "URI=file:mydb.sqlite";
    private string SQL_COUNT_ELEMNTS = "SELECT count(*) FROM Coches;";
    private string SQL_CREATE = "CREATE TABLE IF NOT EXISTS Coches" +
        "(Id INTEGER UNIQUE NOT NULL PRIMARY KEY" +
        ", Color TEXT NOT NULL" +
        ", Modelo TEXT DEFAULT 'Sandero'" +
        ", Marca TEXT DEFAULT 'Dacia');";
    private string[] MARCAS = {"Dacia","Mercedes","Audi","Ferrari","Range","Bendi","Renault","Peugeot","Benly","Porsche"};
    private string[] MODELOS = {"Sandero","A4","Testarrosa","Q3","Q5","Velar","208","Clio","Megane","911"};
    private string[] COLORES = { "Negro", "Blanco", "Rojo", "Azul", "Rosa", "Morado", "Gris", "Marrón", "Verde", "Amarillo" };
    private int NUM_DATA = 1000;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        IDbConnection dbConnection = CreateAndOpenDatabase();
        AddRandomData(dbConnection);


        dbConnection.Close();
        Debug.Log("End");
    }

    private IDbConnection CreateAndOpenDatabase()
    {
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        IDbCommand dbCmd = dbConnection.CreateCommand();
        dbCmd.CommandText = SQL_CREATE;
        dbCmd.ExecuteReader();

        return dbConnection;
    }

    private void AddRandomData(IDbConnection dbConnection)
    {
        if (CountNumberElements(dbConnection) > 0)
        {
            return;
        }
        string command = "INSERT INTO Coches (Color,Modelo,Marca) VALUES ";
        System.Random rnd = new System.Random();
        for (int i = 0; i < NUM_DATA; i++)
        {
            string color = COLORES[rnd.Next(COLORES.Length)];
            string modelo = MODELOS[rnd.Next(MODELOS.Length)]; 
            string marca = MARCAS[rnd.Next(MARCAS.Length)];
            command += $"('{color}','{modelo}','{marca}'),";
        }
        command = command.Remove(command.Length - 1, 1);
        command += ";";
        Debug.Log(command);
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
