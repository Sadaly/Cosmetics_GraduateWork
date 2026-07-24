import React, { useState } from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from './Sidebar';

// NOTE: AuthProvider is NOT re-wrapped here — it's already at App level.
export const DashboardLayout: React.FC = () => {
    const [mobileOpen, setMobileOpen] = useState(false);

    return (
        <div className="flex min-h-screen" style={{ background: 'var(--bg-base)' }}>
            {/* Sidebar */}
            <Sidebar mobileOpen={mobileOpen} onMobileClose={() => setMobileOpen(false)} />

            {/* Main */}
            <div className="flex-1 flex flex-col min-w-0">
                {/* Mobile top bar */}
                <div className="flex items-center justify-between px-4 py-3 lg:hidden"
                    style={{
                        background: 'var(--bg-surface)',
                        borderBottom: '1px solid var(--border-subtle)',
                        boxShadow: 'var(--shadow-sm)',
                    }}>
                    <span style={{
                        fontFamily: "'Cormorant Garamond', Georgia, serif",
                        fontSize: '1.25rem', fontWeight: 600,
                        color: 'var(--color-primary)',
                    }}>
                        Cosmetics
                    </span>
                    <button
                        onClick={() => setMobileOpen(true)}
                        style={{
                            background: 'var(--bg-muted)', border: 'none', borderRadius: 8,
                            width: 36, height: 36, cursor: 'pointer', display: 'flex',
                            alignItems: 'center', justifyContent: 'center',
                            color: 'var(--text-primary)', fontSize: '1.125rem',
                        }}
                    >
                        ☰
                    </button>
                </div>

                {/* Page content */}
                <main className="flex-1 overflow-y-auto p-4 md:p-6 lg:p-8">
                    <div style={{ maxWidth: '1400px', margin: '0 auto' }}>
                        <Outlet />
                    </div>
                </main>
            </div>
        </div>
    );
};
