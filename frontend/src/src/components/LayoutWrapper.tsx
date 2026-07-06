"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";

export default function LayoutWrapper({ children }: { children: React.ReactNode }) {
  const pathname = usePathname() || "";
  const router = useRouter();
  const isLoginPage = pathname === "/login";

  const handleLogout = () => {
    // Remove token cookie
    document.cookie = "auth-token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
    router.push("/login");
  };

  if (isLoginPage) {
    return <main className="min-h-screen bg-muted/20">{children}</main>;
  }

  // Format local date
  const formattedDate = new Date().toLocaleDateString("pt-BR", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });

  return (
    <div className="flex min-h-screen">
      <aside className="hidden w-64 flex-col border-r bg-white/70 px-4 py-6 backdrop-blur sm:flex">
        <div className="mb-8">
          <h1 className="text-lg font-semibold">Apontamento</h1>
          <p className="text-sm text-muted-foreground">Gestão de horas</p>
        </div>
        <nav className="flex flex-1 flex-col gap-2 text-sm">
          <Link
            className={`rounded-md px-3 py-2 font-medium hover:bg-muted ${
              pathname === "/" ? "bg-muted text-foreground" : "text-foreground"
            }`}
            href="/"
          >
            Visão geral
          </Link>
          <Link
            className={`rounded-md px-3 py-2 font-medium hover:bg-muted ${
              pathname.startsWith("/projetos") ? "bg-muted text-foreground" : "text-foreground"
            }`}
            href="/projetos"
          >
            Projetos
          </Link>
          <Link
            className={`rounded-md px-3 py-2 font-medium hover:bg-muted ${
              pathname.startsWith("/apontamentos") ? "bg-muted text-foreground" : "text-foreground"
            }`}
            href="/apontamentos"
          >
            Apontamentos
          </Link>
          <Link
            className={`rounded-md px-3 py-2 font-medium hover:bg-muted ${
              pathname.startsWith("/consultas") ? "bg-muted text-foreground" : "text-foreground"
            }`}
            href="/consultas"
          >
            Consultas
          </Link>
        </nav>
        <button
          onClick={handleLogout}
          className="mt-auto flex w-full items-center justify-start rounded-md px-3 py-2 text-sm font-medium text-red-600 hover:bg-red-50 hover:text-red-700 transition-colors cursor-pointer"
        >
          Sair
        </button>
      </aside>
      <div className="flex flex-1 flex-col">
        <header className="flex h-16 items-center justify-between border-b bg-white/70 px-6 backdrop-blur">
          <div className="flex items-center gap-2">
            <span className="text-sm font-medium">Menu</span>
          </div>
          <div className="flex items-center gap-4">
            <div className="text-sm text-muted-foreground">{formattedDate}</div>
          </div>
        </header>
        <main className="flex-1 bg-muted/20 px-6 py-8">{children}</main>
      </div>
    </div>
  );
}
