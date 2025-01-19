import { useEffect, useState } from "react";
import "./App.css";
import * as signalR from "@microsoft/signalr";

function App() {
  const [forecasts, setForecasts] = useState();

  useEffect(() => {
    console.log("starting connection");
    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Debug) // add this for diagnostic clues
      .withUrl("http://localhost:5288/chatHub", {
        skipNegotiation: true, // skipNegotiation as we specify WebSockets
        transport: signalR.HttpTransportType.WebSockets, // force WebSocket transport
      })
      .build();

    console.log("connection", connection);

    connection
      .start()
      .then(() => {
        console.log("Connected to SignalR hub");
        connection
          .invoke("SendMessage", "gotem chief")
          .catch(() => console.error("it didn't work"));
      })
      .catch((err) => console.error("Error connecting to hub:", err));

    connection.on("ReceiveMessage", (message) => {
      console.log("Received message:", message);
    });
  }, []);

  useEffect(() => {
    populateWeatherData();
  }, []);

  const contents =
    forecasts === undefined ? (
      <p>
        <em>
          Loading... Please refresh once the ASP.NET backend has started. See{" "}
          <a href="https://aka.ms/jspsintegrationreact">
            https://aka.ms/jspsintegrationreact
          </a>{" "}
          for more details.
        </em>
      </p>
    ) : (
      <table className="table table-striped" aria-labelledby="tableLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {forecasts.map((forecast) => (
            <tr key={forecast.date}>
              <td>{forecast.date}</td>
              <td>{forecast.temperatureC}</td>
              <td>{forecast.temperatureF}</td>
              <td>{forecast.summary}</td>
            </tr>
          ))}
        </tbody>
      </table>
    );

  return (
    <div>
      <h1 id="tableLabel">Weather forecast!!!</h1>
      <p>This component demonstrates fetching data from the server.</p>
      {contents}
    </div>
  );

  async function populateWeatherData() {
    const response = await fetch("weatherforecast");
    const data = await response.json();
    setForecasts(data);
  }
}

export default App;
