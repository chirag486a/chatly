import { Link, useLocation } from "react-router-dom";
import DefaultUserProfile from "../../../../assets/default-user-profile.svg";
import { useContext } from "react";
import APIContext from "../../../../Context/APIContext";
export default function Contact({
  contactName,
  contactId,
  lastMessage,
  lastMessageTime,
  profileImageUrl,
}) {
  let location = useLocation();
  const isActive = location.pathname === `/chat/${contactId}`;
  let { setCurrentContact } = useContext(APIContext);
  return (
    <Link
      to={`/chat/${contactId}`}
      className={`contact btn border-none dark:shadow-none h-fit w-full px-1 block rounded-xl ${
        isActive ? " bg-base-200" : " bg-base-100"
      }`}
      onClick={() => setCurrentContact(contactId)}
    >
      <div className="flex items-start px-2 py-4 justify-between gap-3.5 prose prose-p:font-normal">
        <img
          src={profileImageUrl ? profileImageUrl : DefaultUserProfile}
          alt={contactName + " profile picture"}
          className="mb-0 self-center"
        />
        <div className="flex-grow">
          <div className="prose prose-p:text-base prose-p:text-neutral-950 dark:prose-p:text-neutral-50 prose-p:text-left">
            <p>{contactName ? contactName : "Nobiee Nobiee"}</p>
          </div>
          <div className="prose prose-p:text-sm prose-p:text-neutral-500 prose-p:text-left">
            <p>{lastMessage ? lastMessage : "Message"}</p>
          </div>
        </div>
        <div className="self-start mt-1">
          <div className="prose prose-p:text-xs prose-h1:text-right">
            <p>{lastMessageTime ? lastMessageTime : "10:00 AM"}</p>
          </div>
        </div>
      </div>
    </Link>
  );
}
