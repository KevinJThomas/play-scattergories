import { DialogTitle } from "@headlessui/react";
import { XMarkIcon } from "@heroicons/react/24/outline";
import ChatBox from "./ChatBox";
import ChatBubble from "./ChatBubble";
import { useEffect, useRef, useState } from "react";

export default function DialogChatContent({
  setHasUnreadMessage,
  setOpen,
  connection,
  name,
}) {
  const divRef = useRef(null);

  const [messages, setMessages] = useState([]);

  useEffect(() => {
    if (!connection) {
      return;
    }

    connection.on("ChatReceived", (message) => {
      setMessages((prev) => [...prev, message]);
      setHasUnreadMessage(true);
    });

    return () => connection.off("ChatReceived");
  }, [connection, setHasUnreadMessage]);

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
    <div className="flex h-full flex-col overflow-hidden bg-white py-6 shadow-xl">
      <div className="px-4 sm:px-6">
        <div className="flex items-start justify-between">
          <DialogTitle className="text-base font-semibold text-gray-900"></DialogTitle>
          <div className="ml-3 flex h-7 items-center">
            <button
              type="button"
              onClick={() => {
                setHasUnreadMessage(false);
                setOpen(false);
              }}
              className="relative rounded-md bg-white text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
            >
              <span className="absolute -inset-2.5" />
              <span className="sr-only">Close panel</span>
              <XMarkIcon aria-hidden="true" className="size-6" />
            </button>
          </div>
        </div>
      </div>
      <div className="relative mt-6 flex-1 px-4 sm:px-6">
        <div ref={divRef} className="h-[calc(100vh-250px)] overflow-y-scroll">
          {messages.map(({ name, value, id }) => (
            <ChatBubble key={id} message={value} name={name} />
          ))}
        </div>
        <ChatBox connection={connection} name={name} />
      </div>
    </div>
  );
}
