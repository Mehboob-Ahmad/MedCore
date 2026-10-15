import Image from "next/image";
import Link from "next/link";

interface LogoProps {
  className?: string;
  size?: number;
  showText?: boolean;
  textSize?: string;
  href?: string;
  onClick?: () => void;
}

export function Logo({
  className = "",
  size = 36,
  showText = true,
  textSize = "text-xl",
  href,
  onClick,
}: LogoProps) {
  const content = (
    <div className={`inline-flex items-center gap-2.5 ${className}`}>
      <div 
        className="relative overflow-hidden rounded-xl shadow-sm shrink-0 flex items-center justify-center transition-transform hover:scale-105"
        style={{ width: size, height: size }}
      >
        <Image
          src="/logo.png"
          alt="MedicHp Logo"
          width={size}
          height={size}
          className="object-contain w-full h-full rounded-xl"
          priority
        />
      </div>

      {showText && (
        <span className={`font-bold tracking-tight text-[#2A2E75] dark:text-sky-400 ${textSize}`}>
          MedicHp
        </span>
      )}
    </div>
  );

  if (href) {
    return (
      <Link href={href} onClick={onClick} className="inline-flex items-center focus:outline-none">
        {content}
      </Link>
    );
  }

  return content;
}
