import React from 'react';
import {Cropper} from 'react-image-cropper'

import {
    Col,
    Row,
    PageHeader,
    Button,
    Divider
} from 'components';

import { RoutedComponent, connect } from 'routes/routedComponent';
import { CONTENT_VIEW_STATIC } from 'layouts/DefaultLayout/modules/layout';

import classes from './cropImage.scss';

import photo1 from './photos/photo1.jpg';
import photo2 from './photos/photo2.jpg';
import photo3 from './photos/photo3.jpg';
import photo4 from './photos/photo4.jpg';
import photo5 from './photos/photo5.jpg';

class CropImage extends RoutedComponent {
    constructor(props, context) {
        super(props, context);

        this.state = {
            image: '',
            imageLoaded: false,
            image1: '',
            imageL1oaded: false,
            image2: '',
            image2Loaded: false,
            image3: '',
            image3Loaded: false,
            image4: '',
            image4Loaded: false,
            image4BeforeLoaded: false,
            image4Values: ''
        }
    }

    getLayoutOptions() {
        return {
            contentView: CONTENT_VIEW_STATIC
        }
    }

    handleImageLoaded(state){
        this.setState({
            [state + 'Loaded']: true
        });
    }

    handleBeforeImageLoad(state){
        this.setState({
            [state + 'BeforeLoaded']: true
        });
    }

    handleClick(state){
        let node = this.refs[state];
        this.setState({
            [state]: node.crop()
        });
    }

    handleChange(state, values){
        this.setState({
            [state + 'Values']: values
        });
    }

    handleGetValues(state){
        let node = this.refs[state];
        this.setState({
            [state + 'Values']: node.values()
        });
    }

    render() {
        return (
            <div className='p-b-3'>
                <Row>
                    <Col lg={ 12 }>
                        <h4>Default Image</h4>

                        <Cropper
                            src={photo1}
                            ref="image"
                        />
                        <Button
                            bsStyle='default'
                            onClick={() => this.handleClick('image')}
                            className='m-t-2'
                        >
                            Crop
                            <i className='fa fa-crop fa-fw m-l-1'></i>
                        </Button>
                        {   this.state.image && (
                                <div>
                                    <Divider textPosition='center'>Cropped Image</Divider>
                                    <img src={this.state.image} alt=""/>
                                </div>
                            )
                        }
                    </Col>

                    <Col lg={ 12 }>
                        <h4 className='m-t-3'>With given origin X and Y</h4>

                        <Cropper
                            src={photo2}
                            ref="image1"
                            originX={100}
                            originY={100}
                        />
                        <Button
                            bsStyle='default'
                            onClick={() => this.handleClick('image1')}
                            className='m-t-2'
                        >
                            Crop
                            <i className='fa fa-crop fa-fw m-l-1'></i>
                        </Button>
                        {   this.state.image1 && (
                                <div>
                                    <Divider textPosition='center'>Cropped Image</Divider>
                                    <img src={this.state.image1} alt=""/>
                                </div>
                            )
                        }
                    </Col>

                    <Col lg={ 12 }>
                        <h4 className='m-t-3'>With given ratio</h4>
                        <Cropper
                            src={photo3}
                            ref="image2"
                            ratio={16 / 9}
                            width={300}
                        />
                        <Button
                            bsStyle='default'
                            onClick={() => this.handleClick('image2')}
                            className='m-t-2'
                        >
                            Crop
                            <i className='fa fa-crop fa-fw m-l-1'></i>
                        </Button>
                        {   this.state.image2 && (
                                <div>
                                    <Divider textPosition='center'>Cropped Image</Divider>
                                    <img src={this.state.image2} alt=""/>
                                </div>
                            )
                        }
                    </Col>

                    <Col lg={ 12 }>
                        <h4 className='m-t-3'>Disabled</h4>

                        <Cropper
                            src={photo4}
                            ref="image3"
                            disabled
                        />
                    </Col>

                    <Col lg={ 12 }>
                        <h4 className='m-t-3'>All Available Props Used</h4>

                        <p>
                            Variable width and height, cropper frame is relative to natural image size, don't allow new selection, set custom styles
                        </p>

                        <Cropper
                            src={photo5}
                            ref="image4"
                            width={200}
                            height={500}
                            originX={200}
                            originY={50}
                            fixedRatio={false}
                            allowNewSelection={false}
                            onChange={values => this.handleChange('image4', values)}
                            styles={{
                                source_img: {
                                    WebkitFilter: 'blur(3.5px)',
                                    filter: 'blur(3.5px)'
                                },
                                modal: {
                                    opacity: 0.5,
                                    backgroundColor: '#fff'
                                },
                                dotInner: {
                                    borderColor: '#ff0000'
                                },
                                dotInnerCenterVertical: {
                                    backgroundColor: '#ff0000'
                                },
                                dotInnerCenterHorizontal: {
                                    backgroundColor: '#ff0000'
                                }
                            }}
                        />
                        <Button
                            bsStyle='default'
                            onClick={() => this.handleClick('image4')}
                            className='m-t-2'
                        >
                            Crop
                            <i className='fa fa-crop fa-fw m-l-1'></i>
                        </Button>
                        {   this.state.image4 && (
                                <div>
                                    <Divider textPosition='center'>Cropped Image</Divider>
                                    <img src={this.state.image4} alt=""/>
                                </div>
                            )
                        }
                    </Col>
                </Row>
            </div>
        );
    }
}

export default connect()(CropImage);
