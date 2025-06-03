import Search from "../../../../assets/search.svg?react";
import TrippleDots from "../../../../assets/tripple-dots.svg?react";
export default function ChatWindowTitle({ name, lastSeen }) {
  return (
    <div className="flex items-center justify-between">
      <div>
        <div className="prose prose-p:text-4xl prose-p:mb-3 prose-p:font-semibold">
          <p>{name ? name : "Ethan Carter"}</p>
        </div>
        <div className="prose prose-p:text-sm prose-p:text-neutral-400">
          <p>{lastSeen ? lastSeen : "last seen 23 min ago"}</p>
        </div>
      </div>
      <div className="flex gap-4 self-start">
        <button className="btn shadow-none border-none w-fit h-fit p-2.5 bg-neutral-100 rounded-full">
          <Search className="h-5 w-5" />
        </button>
        <button className="btn shadow-none border-none w-fit h-fit p-2.5 bg-neutral-100 rounded-full">
          <TrippleDots className="h-5 w-5 fill-base-content" />
        </button>
      </div>
    </div>
  );
}
