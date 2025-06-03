import GirlWithLaptopSittingInSofa from "../../../../assets/girl-with-laptop-sitting-in-sofa.svg?react";
function FirstTime() {
  return (
    <div className="flex flex-col items-center justify-center mt-10">
      <div className="prose prose-h3:text-3xl prose-h3:mb-3 text-center w-full flex flex-col items-center justify-center prose-p:mt-1">
        <h3>Welcome to Chatly</h3>
        <p>Select a contact to start chatting</p>
      </div>
    </div>
  );
}
export default function NoChatSelection({ noContacts }) {
  if (noContacts) return <FirstTime />;
  return (
    <div className="flex flex-col items-center justify-center">
      <GirlWithLaptopSittingInSofa className="mb-8" />
      <div className="prose prose-h3:text-lg prose-h3:mb-3 text-center w-full flex flex-col items-center justify-center prose-p:text-sm prose-p:mt-1">
        <h3>Select a chat to start messaging</h3>
        <p>Pick a conversation from your list to begin exchanging messages.</p>
      </div>
    </div>
  );
}
