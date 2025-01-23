import { useEffect, useRef } from "react";
import ChatBubble from "./ChatBubble";
import ChatBox from "./ChatBox";

export default function ChatContent({ messages, connection, name }) {
  const divRef = useRef(null);

  const handleScroll = () => {
    if (divRef.current) {
      // console.log(
      //   divRef.current.scrollTop + divRef.current.offsetHeight >=
      //     divRef.current.scrollHeight,
      // );
      divRef.current.scrollTop = divRef.current.scrollHeight;

      // console.log(
      //   divRef.current.scrollTop + divRef.current.offsetHeight >=
      //     divRef.current.scrollHeight,
      // );
    }
  };

  useEffect(() => {
    handleScroll();
  }, [messages]);

  return (
    <div className="relative mt-6 flex-1 px-4 sm:px-6">
      <div ref={divRef} className="h-[calc(100vh-250px)] overflow-y-scroll">
        {messages.map(({ name, value, id }) => (
          <ChatBubble key={id} message={value} name={name} />
        ))}
      </div>
      <ChatBox connection={connection} name={name} />
    </div>
  );
}
