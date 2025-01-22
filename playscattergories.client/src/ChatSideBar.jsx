import { Dialog, DialogPanel } from "@headlessui/react";
import DialogChatContent from "./DialogChatContent";

export default function ChatSideBar({
  open,
  setOpen,
  connection,
  name,
  setHasUnreadMessage,
}) {
  return (
    <Dialog
      open={open}
      onClose={() => {
        console.log("onClose()");
        setOpen(false);
        setHasUnreadMessage(false);
      }}
      className="relative z-10"
      unmount={false}
    >
      <div className="fixed inset-0" />

      <div className="fixed inset-0 overflow-hidden">
        <div className="absolute inset-0 overflow-hidden">
          <div className="pointer-events-none fixed inset-y-0 right-0 flex max-w-full pl-10">
            <DialogPanel
              transition
              className="pointer-events-auto w-screen max-w-md transform transition duration-500 ease-in-out data-[closed]:translate-x-full sm:duration-700"
            >
              <DialogChatContent
                setHasUnreadMessage={setHasUnreadMessage}
                setOpen={setOpen}
                connection={connection}
                name={name}
              />
            </DialogPanel>
          </div>
        </div>
      </div>
    </Dialog>
  );
}
