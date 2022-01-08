import React, { useState, useEffect, useCallback } from "react";
import { useDropzone } from "react-dropzone";
import axios from "axios";
import {Button} from '@mui/material'

const thumbsContainer = {
    display: "flex",
    flexDirection: "row",
    flexWrap: "wrap",
    marginTop: 16,
    padding: 20
  };
  
  const thumb = {
    position: "relative",
    display: "inline-flex",
    borderRadius: 2,
    border: "1px solid #eaeaea",
    marginBottom: 8,
    marginRight: 8,
    width: 100,
    height: 100,
    padding: 4,
    boxSizing: "border-box"
  };
  
  const thumbInner = {
    display: "flex",
    minWidth: 0,
    overflow: "hidden"
  };
  
  const img = {
    display: "block",
    width: "auto",
    height: "100%"
  };
  
  const thumbButton = {
    position: "absolute",
    right: 10,
    bottom: 10
  };

  export default function ImageUpload() {
    const [files, setFiles] = useState([]); 

    const { getRootProps, getInputProps } = useDropzone({
      accept: "image/*",
      onDrop: (acceptedFiles) => {
        setFiles(
          acceptedFiles.map((file) =>          
            Object.assign(file, {
              preview: URL.createObjectURL(file)
            })
          )
        );
      }
    });
  
    const thumbs = files.map((file, index) => (
      <div style={thumb} key={file.name}>
        <div style={thumbInner}>
          <img src={file.preview} style={img} alt="" />
        </div>
        <button
          style={thumbButton}         
        >
          Remove
        </button>
      </div>
    ));
  
    useEffect(
      () => () => {
        // Make sure to revoke the Object URL to avoid memory leaks
        files.forEach((file) => URL.revokeObjectURL(file.preview));
      },
      [files]
    );

    async function UploadImages() {
        console.log(files)        
        const formData = new FormData();
        files.length > 0 && files.forEach((file) => {
          formData.append('file', file)
        })
        await axios.post('/api/storage/upload-images', formData)
        .then( (response ) => {
            console.log(response);
        })
    }
  
    return (
      <section className="container">
        <div {...getRootProps({ className: "dropzone" })}>
          <input {...getInputProps()} />
          <p>Drag 'n' drop some files here, or click to select files</p>
        </div>       
        <aside style={thumbsContainer}>{thumbs}</aside>
        <Button onClick={UploadImages}>Upload</Button>
      </section>
    );
  }