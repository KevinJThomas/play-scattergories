import { useEffect } from "react";
import * as signalR from "@microsoft/signalr";

function App() {
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

  return <h1 className="text-3xl font-bold underline">Hello world!</h1>;
}

export default App;
