export default function MessageInputField() {
  return (
    <div className="px-24">
      <div className="join w-full bg-neutral-50 rounded-2xl items-center px-2">
        <div className="w-full">
          <label className="input bg-transparent focus:outline-0 focus-within:shadow-none focus:shadow-none  border-0 focus-within:outline-0 validator join-item w-full">
            <input type="text" placeholder="Type a message..." required />
          </label>
        </div>
        <button className="btn btn-primary h-fit w-fit join-item text-sm items-center justify-center rounded-full py-1.5 px-4">
          <span className="self-center text-sm font-normal max-h-min">Send</span>
        </button>
      </div>
    </div>
  );
}
