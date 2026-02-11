import type { Metadata } from "next";
import Link from "next/link";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Apontamento de Horas",
  description: "Sistema de apontamento de horas",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="pt-BR">
      <body
        className={`${geistSans.variable} ${geistMono.variable} min-h-screen bg-background text-foreground antialiased`}
      >
        <div className="flex min-h-screen">
          <aside className="hidden w-64 flex-col border-r bg-white/70 px-4 py-6 backdrop-blur sm:flex">
            <div className="mb-8">
              <h1 className="text-lg font-semibold">Apontamento</h1>
              <p className="text-sm text-muted-foreground">Gestão de horas</p>
            </div>
            <nav className="flex flex-1 flex-col gap-2 text-sm">
              <Link
                className="rounded-md px-3 py-2 font-medium text-foreground hover:bg-muted"
                href="/"
              >
                Visão geral
              </Link>
              <Link
                className="rounded-md px-3 py-2 font-medium text-foreground hover:bg-muted"
                href="/projetos"
              >
                Projetos
              </Link>
              <Link
                className="rounded-md px-3 py-2 font-medium text-foreground hover:bg-muted"
                href="/apontamentos"
              >
                Apontamentos
              </Link>
              <Link
                className="rounded-md px-3 py-2 font-medium text-foreground hover:bg-muted"
                href="/consultas"
              >
                Consultas
              </Link>
            </nav>
            <div className="text-xs text-muted-foreground">v0.1</div>
          </aside>
          <div className="flex flex-1 flex-col">
            <header className="flex h-16 items-center justify-between border-b bg-white/70 px-6 backdrop-blur">
              <div className="flex items-center gap-2">
                <span className="text-sm font-medium">Menu</span>
              </div>
              <div className="text-sm text-muted-foreground">
                10 de fevereiro de 2026
              </div>
            </header>
            <main className="flex-1 bg-muted/20 px-6 py-8">{children}</main>
          </div>
        </div>
      </body>
    </html>
  );
}
