import Avatar from "../Avatar";
import Button from "../Button";
import Card from "../Card";
import Header from "../Header";

const people = [
  {
    name: "Leslie Alexander",
  },
  {
    name: "Michael Foster",
  },
  {
    name: "Dries Vincent",
  },
  {
    name: "Lindsay Walton",
  },
  {
    name: "Courtney Henry",
  },
  {
    name: "Tom Cook",
  },
  {
    name: "Jimmy Dean",
  },
  {
    name: "Pablo",
  },
];

export default function Lobby() {
  return (
    <div>
      <Header>Lobby</Header>
      <Card>
        <ul role="list" className="divide-y divide-gray-200">
          {people.map((person) => (
            <li key={person.email} className="flex gap-x-4 py-1">
              <div className="flex items-center">
                <Avatar name={person.name} />
                <p className="text-sm/6 text-gray-900">{person.name}</p>
              </div>
            </li>
          ))}
        </ul>
        <Button fullWidth>Start Game</Button>
      </Card>
    </div>
  );
}
