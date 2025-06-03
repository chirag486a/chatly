export default function Chat({ isLeft, imgSrc, name, message }) {
  return (
    <div className={`chat ${isLeft ? "chat-start" : "chat-end"}`}>
      <div className="chat-image avatar">
        <div className="w-10 rounded-full">
          <img
            alt="Tailwind CSS chat bubble component"
            src={
              imgSrc
                ? imgSrc
                : "https://img.daisyui.com/images/profile/demo/kenobee@192.webp"
            }
          />
        </div>
      </div>
      <div className="chat-header text-neutral-400">
        {name ? name : "Noobie"}
      </div>
      <div className="chat-bubble">
        {message ? message : "Sent you a message"}
      </div>
    </div>
  );
}
