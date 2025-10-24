

      # - name: Log in to GHCR
      #   if: github.event_name != 'pull_request'
      #   uses: docker/login-action@v3
      #   with:
      #     registry: ${{ env.REGISTRY }}
      #     username: ${{ github.actor }}
      #     password: ${{ secrets.GITHUB_TOKEN }}


    #      - name: Inject executionRoleArn from secret
    #        run: |
    #          sed -i 's|__EXECUTION_ROLE_ARN__|${{ secrets.ECS_EXECUTION_ROLE_ARN }}|g' .aws/ecs-task-definition.json
    #          sed -i 's|__TASK_ROLE_ARN__|${{ secrets.ECS_TASK_ROLE_ARN }}|g' .aws/ecs-task-definition.json