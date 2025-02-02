import Avatar from "./Avatar";

export default function ChatBubble({ message, name }) {
  return (
    <div className="mb-2 flex items-start gap-1 md:gap-2.5">
      <Avatar name={name} />
      <div className="leading-1.5 flex w-full max-w-[320px] flex-col rounded-e-xl rounded-es-xl border-gray-200 bg-gray-100 p-2 md:p-4">
        <div className="flex items-center space-x-1 md:space-x-2 rtl:space-x-reverse">
          <span className="text-sm font-semibold text-gray-900">{name}</span>
          <span className="text-sm font-normal text-gray-500">
            {new Date().toLocaleTimeString([], {
              hour: "2-digit",
              minute: "2-digit",
              hour12: false,
            })}
          </span>
        </div>
        <p className="py-1 text-sm font-normal text-gray-900 md:py-2.5">
          {message}
        </p>
      </div>
    </div>
  );
}
