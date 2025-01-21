import { ChatBubbleBottomCenterIcon } from "@heroicons/react/20/solid";

export default function ChatButton({ onClick }) {
  return (
    <button
      onClick={onClick}
      type="button"
      className="mt-3 rounded-full bg-pink-600 p-2 text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 md:absolute md:right-3 md:top-3 md:mt-0"
    >
      <ChatBubbleBottomCenterIcon aria-hidden="true" className="size-5" />
    </button>
  );
}
