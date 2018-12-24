﻿class TaskDiagramArea extends React.Component {
    state = {
        hasError: false,
        error: null,
        errorInfo: null
    };

    componentDidMount() {
        this.refs.serializeGraph.state = {
            diagram: this.refs.diagram,
            onClickHandler: this.refs.diagram.SerializeGraph
        };

        this.refs.runGraph.state = {
            diagram: this.refs.diagram,
            onClickHandler: this.refs.diagram.RunGraph
        };
    }

    componentDidCatch(error, info) {
        // Display fallback UI
        this.setState({
            hasError: true,
            error: error,
            errorInfo: info
        });
    }

    render() {
        if (this.state.hasError) {
            return (
                <div style={{ padding: '10px 15px' }}>
                    <h2>Something went wrong.</h2>
                    <div style={{ whiteSpace: 'pre-wrap' }}>
                        {this.state.error && this.state.error.toString()}
                        <br />
                        {this.state.errorInfo.componentStack}
                    </div>
                    <br />
                    <h4>Props:</h4>
                    <pre>{JSON.stringify(this.props, null, 4)}</pre>
                </div>
            );
        }

        return (
            <div className="task-builder-area">
                <div className="task-builder-buttons">
                    <DiagramButton ref="serializeGraph" text="Serialize Graph" />
                    <DiagramButton ref="runGraph" text="Run Graph" />
                </div>
                <div className="task-builder-row">
                    <FunctionTray functions={this.props.functions} />
                    <TaskDiagram ref="diagram" functions={this.props.functions} graph={this.props.graph} />
                </div>
            </div>
        );
    }
}