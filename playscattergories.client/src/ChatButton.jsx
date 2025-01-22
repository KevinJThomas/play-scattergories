import { ChatBubbleBottomCenterIcon } from "@heroicons/react/20/solid";
import clsx from "clsx";

export default function ChatButton({ onClick, hasUnreadMessage }) {
  return (
    <>
      <div className="mt-3 md:absolute md:right-3 md:top-3 md:mt-0">
        {hasUnreadMessage && (
          <span className="absolute">
            <span className="relative flex h-3 w-3">
              <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-green-400 opacity-75"></span>
              <span className="relative inline-flex h-3 w-3 rounded-full bg-green-500"></span>
            </span>
          </span>
        )}
        <button
          onClick={onClick}
          type="button"
          className="rounded-full bg-pink-600 p-2 text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
        >
          <ChatBubbleBottomCenterIcon aria-hidden="true" className="size-5" />
        </button>
      </div>
    </>
  );
}
