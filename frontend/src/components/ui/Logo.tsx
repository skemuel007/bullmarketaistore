import { Cpu } from "lucide-react";
import { Link } from "react-router-dom";

export function Logo() {
  return (
    <Link
      to="/"
      className="group flex items-center gap-2.5"
      aria-label="BullMarket AI Store home"
    >
      <span className="grid size-10 place-items-center rounded-xl bg-gradient-to-br from-electric-300 to-violet-400 text-ink-950 shadow-glow">
        <Cpu size={21} />
      </span>
      <span className="font-display text-xl font-bold tracking-[-.04em] text-white">
        BullMarket<span className="text-electric-300"> AI Store</span>
      </span>
    </Link>
  );
}
