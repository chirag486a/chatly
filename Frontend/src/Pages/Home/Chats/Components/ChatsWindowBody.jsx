import { useEffect, useRef, useState } from "react";
import Chat from "./Chat";

import CopyIcon from "../../../../assets/copy-icon.svg?react";
import DeleteIcon from "../../../../assets/delete-icon.svg?react";
import ForwardIcon from "../../../../assets/forward-icon.svg?react";
import ReplyIcon from "../../../../assets/reply-icon.svg?react";

export default function ChatsWindowBody() {
  const containerRef = useRef(null);

  useEffect(() => {
    const el = containerRef.current;
    if (el) {
      el.scrollTop = el.scrollHeight;
    }
  }, []);

  const [contextMenu, setContextMenu] = useState({
    visible: false, // Should the menu be shown?
    x: 0, // X position on screen
    y: 0, // Y position on screen
    messageId: null, // Which message was clicked?
  });
  return (
    <div
      className="h-[460px] overflow-y-scroll"
      ref={containerRef}
      onContextMenu={(e) => {
        e.preventDefault();
        if (e.target.closest(".chat-bubble") !== null)
          setContextMenu({
            visible: true,
            x: e.pageX,
            y: e.pageY,
          });
      }}
      onClick={() => setContextMenu({ ...contextMenu, visible: false })}
    >
      <Chat isLeft={true} message={"Hello man!😒"} />
      <Chat isLeft={false} message={"Yes man!😒"} />
      <Chat isLeft={true} message={"Hello man!😒"} />
      <Chat isLeft={false} message={"Yes man!😒"} />
      <Chat isLeft={true} message={"Hello man!😒"} />
      <Chat isLeft={false} message={"Yes man!😒"} />
      <Chat isLeft={true} message={"Hello man!😒"} />
      <Chat isLeft={false} message={"Yes man!😒"} />
      <Chat isLeft={true} message={"Hello man!😒"} />
      <Chat isLeft={false} message={"Yes man!😒"} />
      <Chat isLeft={true} message={"Hello man!😒"} />
      <Chat isLeft={false} message={"Yes man!😒"} />
      <Chat isLeft={true} message={"Hello man!😒"} />
      <Chat isLeft={false} message={"Yes man!😒"} />

      {contextMenu.visible && (
        <div
          className="absolute bg-neutral-200 rounded shadow-md z-50"
          style={{ top: contextMenu.y, left: contextMenu.x }}
        >
          <ul className="text-sm">
            <li className="hover:bg-gray-100 cursor-pointer flex items-center">
              <span className="px-4 py-3 flex items-center justify-center">
                <ReplyIcon className="h-4 w-4" />
              </span>
              <p>Reply</p>
            </li>
            <li className="hover:bg-gray-100 cursor-pointer flex items-center pr-6">
              <span className="px-4 py-3 flex items-center justify-center">
                <CopyIcon className="h-4 w-4" />
              </span>
              <p>Edit</p>
            </li>
            <li className="hover:bg-gray-100 cursor-pointer flex items-center pr-6">
              <span className="px-4 py-3 flex items-center justify-center">
                <CopyIcon className="h-4 w-4" />
              </span>
              <p>Copy text</p>
            </li>
            <li className="hover:bg-gray-100 cursor-pointer flex items-center pr-6">
              <span className="px-4 py-3 flex items-center justify-center">
                <ForwardIcon className="h-4 w-4" />
              </span>
              <p>Forward</p>
            </li>
            <li className="hover:bg-gray-100 cursor-pointer flex items-center pr-6">
              <span className="px-4 py-3 flex items-center justify-center">
                <DeleteIcon className="h-4 w-4" />
              </span>
              <p>Delete</p>
            </li>
          </ul>
        </div>
      )}
    </div>
  );
}
