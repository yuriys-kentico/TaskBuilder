﻿class BaseCallerIconWidget extends SRD.BaseWidget {
    constructor(props) {
        super("port-icon", props);
    }

    state = {
        selected: false
    };

    getClassName() {
        return "port " + super.getClassName() + (this.state.selected ? this.bem("-selected") : "");
    }

    select = (select) => {
        this.setState({ selected: select });
    }

    getIcon() {
        if (this.state.selected) {
            return "icon-chevron-right-circle";
        }

        if (this.props.port.isLinked()) {
            return "icon-caret-right";
        }

        return "icon-chevron-right";
    }

    render() {
        return (
            <div
                {...this.getProps()}
                onMouseEnter={() => this.select(true)}
                onMouseLeave={() => this.select(false)}
                data-name={this.props.port.name}
                data-nodeid={this.props.port.getParent().getID()}
            >
                <i className={this.getIcon()} />
            </div>
        );
    }
}