import Avatar from "./Avatar";
import { useState } from "react";

export default function ChatBox({ connection, name = "Unknown User" }) {
  const [text, setText] = useState("");

  function onSubmit(e) {
    e.preventDefault();

    connection.invoke("SendChat", text);

    setText("");
  }

  return (
    <div className="absolute bottom-1 left-6 right-12 flex items-start space-x-3">
      <div className="shrink-0">
        <Avatar name={name} />
      </div>
      <div className="min-w-0 flex-1">
        <form action="#" className="relative" onSubmit={onSubmit}>
          <div className="rounded-lg bg-white outline outline-1 -outline-offset-1 outline-gray-300 focus-within:outline focus-within:outline-2 focus-within:-outline-offset-2 focus-within:outline-indigo-600">
            <label htmlFor="comment" className="sr-only">
              Add your comment
            </label>
            <textarea
              autoFocus
              id="comment"
              name="comment"
              rows={3}
              placeholder="Add your comment..."
              className="block w-full resize-none bg-transparent px-3 py-1.5 text-base text-gray-900 placeholder:text-gray-400 focus:outline focus:outline-0 sm:text-sm/6"
              value={text}
              required
              onChange={(e) => setText(e.target.value)}
            />

            {/* Spacer element to match the height of the toolbar */}
            <div aria-hidden="true" className="py-2">
              {/* Matches height of button in toolbar (1px border + 36px content height) */}
              <div className="py-px">
                <div className="h-9" />
              </div>
            </div>
          </div>

          <div className="absolute inset-x-0 bottom-0 flex justify-between py-2 pl-3 pr-2">
            <div className="shrink-0">
              <button
                type="submit"
                className="inline-flex items-center rounded-md bg-pink-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
              >
                Post
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
