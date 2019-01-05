﻿const SRD = window["storm-react-diagrams"];

interface BasePortWidgetState { }

class BasePortWidget extends SRD.BaseWidget {
    constructor(props) {
        super("srd-default-port", props);
    }

    getClassName() {
        return super.getClassName() + (this.props.model.in ? this.bem("--in") : this.bem("--out"));
    }

    render() {
        var port = <SRD.PortWidget node={this.props.model.getParent()} name={this.props.model.name} />;
        var label = <div className="name">{this.props.model.label}</div>;

        return (
            <div {...this.getProps()}>
                {this.props.model.in ? port : label}
                {this.props.model.in ? label : port}
                {this.props.model.type}
            </div>
        );
    }
}