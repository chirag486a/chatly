import MessageInputField from "./Components/MessageInputField";
import NoChatSelection from "./Components/NoChatSelection";
import ChatWindowTitle from "./Components/ChatWindowTitle";
import ChatsWindowBody from "./Components/ChatsWindowBody";

export default function ChatsWindow({ noChatSelection, noContacts }) {
  if (noContacts) {
    return <NoChatSelection noContacts={noContacts} />;
  }
  if (noChatSelection) {
    return <NoChatSelection noContacts={false} />;
  }
  return (
    <div className="h-full w-[926px]">
      <div className="flex flex-col px-6 h-full gap-6">
        <ChatWindowTitle />
        <div className="flex-grow h-full">
          <ChatsWindowBody />
        </div>
        <MessageInputField />
      </div>
    </div>
  );
}
