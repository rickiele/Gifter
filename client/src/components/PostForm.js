import React, { useContext, useEffect } from "react";
import { PostContext } from "../providers/PostProvider";
import { Button, Form, Label, Input, FormGroup } from 'reactstrap';

const PostForm = () => {
    const { posts, getAllPosts } = useContext(PostContext);

    return (
        <div class="addPostForm">
            <Form>
                <FormGroup>
                    <h2>Add a Post</h2>
                    <Label for="postTitle">Post Title</Label>
                    <Input type="textArea" name="postTitle" id="postTitle" placeholder="insert title" />
                </FormGroup>
                <Button class="formBtn">Add</Button>
            </Form>
        </div>
    )

}

export default PostForm;