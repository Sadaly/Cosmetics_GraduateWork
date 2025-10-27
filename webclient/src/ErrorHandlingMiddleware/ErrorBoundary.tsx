// ErrorBoundary.tsx
import React from "react";
import { ErrorService } from "./ErrorService";

interface Props {
    children: React.ReactNode;
}

interface State {
    hasError: boolean;
}

class ErrorBoundary extends React.Component<Props, State> {
    state: State = { hasError: false };

    static getDerivedStateFromError() {
        return { hasError: true };
    }

    componentDidCatch(error: Error) {
        ErrorService.showError(error);
    }

    render() {
        if (this.state.hasError) {
            return <div style={{ padding: "2rem" }}>Что-то пошло не так 😢</div>;
        }
        return this.props.children;
    }
}

export default ErrorBoundary;
